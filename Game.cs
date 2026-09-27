using System.Text.Json;

namespace TicTacToe;

/// <summary>
/// Base class for every game type. Coordinates a game between two players on one
/// or more boards: whose turn it is, the move history, undo/redo and saving.
///
/// Playing a move follows the Template Method pattern: <see cref="Play"/> fixes
/// the steps (check the cell, apply the move, record it, evaluate the result) and
/// each concrete game supplies its own rules through <see cref="IsLegal"/>,
/// <see cref="Apply"/> and <see cref="IGame.PlayMove"/>.
///
/// This class does not itself declare <see cref="IGame"/>; each concrete game
/// implements that interface, using the members provided here to satisfy it.
/// </summary>
public abstract class Game : IGameView
{
    /// <summary>
    /// The two players in the game. Player one moves first.
    /// </summary>
    public IPlayer PlayerOne { get; }

    public IPlayer PlayerTwo { get; }

    /// <summary>
    /// The commands that have been played, most recent on top (the Command
    /// pattern's history). Undoing pops from here; a fresh move pushes onto it.
    /// </summary>
    private readonly Stack<ICommand> _undo = new();

    /// <summary>
    /// Commands that have been undone and can be redone, most recent on top. A
    /// fresh move clears this, since redoing onto a board that has moved on no
    /// longer makes sense.
    /// </summary>
    private readonly Stack<ICommand> _redo = new();

    /// <summary>
    /// Initialises the shared game state. Called by subclass constructors.
    /// </summary>
    /// <param name="playerOne">The first player.</param>
    /// <param name="playerTwo">The second player.</param>
    /// <param name="boards">The boards the game is played on; at least one.</param>
    protected Game(IPlayer playerOne, IPlayer playerTwo, params Board[] boards)
    {
        if (boards.Length == 0)
        {
            throw new ArgumentException("A game needs at least one board.", nameof(boards));
        }

        PlayerOne = playerOne;
        PlayerTwo = playerTwo;
        _boards = boards;
    }

    /// <summary>Which kind of game this is.</summary>
    public abstract GameType Type { get; }

    /// <summary>The boards the game is played on. Most games have just one.</summary>
    private readonly Board[] _boards;

    public IReadOnlyList<Board> Boards => _boards;

    /// <summary>Every move played so far, most recent on top.</summary>
    private readonly Stack<Placement> _history = new();

    /// <summary>Every move played so far, oldest first.</summary>
    public IEnumerable<Placement> History => _history.Reverse();

    public int MoveCount => _history.Count;

    /// <summary>
    /// The player whose turn it is right now. Players alternate, so this follows
    /// from how many moves have been played.
    /// </summary>
    public IPlayer CurrentPlayer => MoveCount % 2 == 0 ? PlayerOne : PlayerTwo;

    public IPlayer OtherPlayer => MoveCount % 2 == 0 ? PlayerTwo : PlayerOne;

    // Move Template //



    /// <summary>
    /// Every legal placement for the player whose turn it is. By default, every
    /// empty cell on every board, using the board's next number as the value.
    /// Games override this when some empty cells aren't playable (e.g. dead
    /// boards) or when a move involves more than a cell (e.g. a chosen number).
    /// </summary>
    /// <returns>The legal placements, built fresh on each call.</returns>
    public virtual IReadOnlyList<Placement> CandidateMoves() =>
            Enumerable.Range(0, _boards.Length)
                .SelectMany(b => _boards[b].EmptyCells()
                    .Select(c => new Placement(c.Row, c.Column, _boards[b].NextNumber, b)))
                .ToList();

    /// <summary>
    /// Tests what playing a placement would lead to, without keeping it: the
    /// move is played through the normal rules (<see cref="Play"/>) and then taken
    /// back (<see cref="Unplay"/>), so the board, history and any game state end
    /// as they started. For computer player to look for winning moves.
    /// </summary>
    /// <param name="p">The placement to try.</param>
    /// <returns>
    /// The outcome the move would have, or <see cref="MoveOutcome.Illegal"/> if
    /// it isn't allowed (in which case nothing was changed). <returns>
    public MoveOutcome Preview(Placement p)
        {
            MoveOutcome outcome = Play(p);
            if (outcome != MoveOutcome.Illegal)
                Unplay();
            return outcome;
        }



    /// <summary>
    /// Asks the player to turn their chosen cell into a full move, using this game's
    /// legal options (for example a number, in Numerical Tic Tac Toe).
    /// </summary>
    /// <param name="player"></param>
    /// <param name="move"></param>
    /// <returns></returns>
    public Placement CreatePlacement(IPlayer player, Move move) =>
        player.ChoosePlacement(this, move);

    /// <summary>
    /// applies a placement using the command patern, returns illegal if unable. Pushes command to undo stack, and returns game outcome (i.e. win)
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public MoveOutcome MakeMove(Placement p)
    {
        var command = new MoveCommand(this, p.Row, p.Column, p.SelectedNumber, p.BoardIndex);

        if (!command.Execute())
            return MoveOutcome.Illegal;

        _undo.Push(command);
        _redo.Clear();
        return command.Outcome;
    }

    /// <summary>
    /// Plays a move: checks it, applies it, records it and evaluates the result.
    /// Returns <see cref="MoveOutcome.Illegal"/> (without changing anything) if
    /// the cell is taken or the game's rules forbid the move.
    /// </summary>
    /// 
    internal MoveOutcome Play(Placement placement)
    {
        // The rules live on IGame, which each concrete game implements. //
        if (this is not IGame rules)
        {
            throw new InvalidOperationException($"{GetType().Name} must implement IGame.");
        }

        if (!IsOnAnEmptyCell(placement) || !rules.IsLegal(placement))
        {
            return MoveOutcome.Illegal;
        }

        rules.Apply(placement);
        _history.Push(placement);
        var result = rules.PlayMove(placement);

        return result;
    }


    /// <summary>
    /// Reverses the last move made. Returns it, or null if there were no moves.
    /// </summary>
    internal Placement? Unplay()
    {
        if (!_history.TryPop(out Placement last))
        {
            return null;
        }

        _boards[last.BoardIndex].UndoLastMove();
        Unapply(last);
        return last;
    }
    /// <summary>
    /// Hook to allow games to reverse state when a move is undone, called when needed. 
    /// </summary>
    /// <param name="p"></param>
    protected virtual void Unapply(Placement p) { }

    /// <summary>True if the placement names a real board and an empty cell on it.</summary>
    private bool IsOnAnEmptyCell(Placement p)
    {
        if (p.BoardIndex < 0 || p.BoardIndex >= _boards.Length)
        {
            return false;
        }

        Board board = _boards[p.BoardIndex];
        return board.IsInBounds(p.Row, p.Column) && board.GetCell(p.Row, p.Column) is null;
    }

    /// <summary>
    /// Every straight run of <paramref name="length"/> cells on the board: across
    /// rows, down columns and along both diagonal directions.
    /// </summary>
    protected static IEnumerable<(int Row, int Column)[]> Lines(Board board, int length)
    {
        (int Row, int Column)[] directions = { (0, 1), (1, 0), (1, 1), (1, -1) };

        for (int row = 0; row < board.Height; row++)
        {
            for (int column = 0; column < board.Width; column++)
            {
                foreach ((int dRow, int dColumn) in directions)
                {
                    var line = new (int Row, int Column)[length];
                    for (int i = 0; i < length; i++)
                    {
                        line[i] = (row + i * dRow, column + i * dColumn);
                    }

                    if (board.IsInBounds(line[^1].Row, line[^1].Column))
                    {
                        yield return line;
                    }
                }
            }
        }
    }

    // Commands (undo / redo) //

    /// <summary>True if there is a command that can be undone.</summary>
    public bool CanUndo => _undo.Count > 0;

    /// <summary>True if there is an undone command that can be redone.</summary>
    public bool CanRedo => _redo.Count > 0;

    /// <summary>
    /// Undoes the most recent command and moves it onto the redo history.
    /// Returns the undone move, or null if there was nothing to undo.
    /// </summary>
    public Placement? Undo()
    {
        if (_undo.Count == 0)
        {
            return null;
        }

        ICommand command = _undo.Pop();
        command.Undo();
        _redo.Push(command);

        return (command as MoveCommand)?.Placement;
    }

    /// <summary>
    /// Redoes the most recently undone command: re-executes it and moves it back
    /// onto the undo history.
    /// Returns the redone move, or null if there was nothing to redo.
    /// </summary>
    public Placement? Redo()
    {
        if (_redo.Count == 0)
        {
            return null;
        }

        ICommand command = _redo.Pop();
        command.Execute();
        _undo.Push(command);

        return (command as MoveCommand)?.Placement;
    }

    // Saving and loading //

    /// <remarks>
    /// Players are rebuilt with their saved kinds (human or computer), and every
    /// saved move is replayed through <see cref="MakeMove"/>, so loaded moves can be undone.
    /// </remarks>
    public static IGame Load(string data)
    {
        GameState? state = JsonSerializer.Deserialize<GameState>(data, SerializerOptions);

        if (state is null)
        {
            throw new ArgumentException("The game state is not valid JSON.", nameof(data));
        }

        IGame game = GameFactory.CreateGame(
            state.GameType,
            PlayerFactory.Create(state.PlayerOneKind, state.PlayerOne),
            PlayerFactory.Create(state.PlayerTwoKind, state.PlayerTwo),
            state.BoardSize);

        foreach (Placement placement in state.Moves)
        {
            if (game.MakeMove(placement) == MoveOutcome.Illegal)
            {
                throw new InvalidDataException($"The saved move {placement} is not legal.");
            }
        }

        return game;
    }

    /// <summary>Persists the current game so it can be reloaded later.</summary>
    /// <remarks>
    /// Prompts at the console for a save name and writes the current
    /// <see cref="State"/> to "&lt;name&gt;.json" in the Save games folder.
    /// </remarks>
    public void Save()
    {
        Console.Write("Save as (name, no extension): ");
        string? name = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("No name given; not saved.");
            return;
        }

        string path = SaveFilePath(name);
        File.WriteAllText(path, State);
        Console.WriteLine($"Saved to {path}.");
    }


    /// <summary>
    /// Creates separate folder for save files outside of working directory ///
    /// </summary>
    public static string SaveFolder => Path.Combine(Directory.GetCurrentDirectory(), "Saved Games");

    /// <summary>
    /// The path a save with the given name is written to and loaded from:
    /// "&lt;name&gt;.json" in the current working directory.
    /// </summary>
    public static string SaveFilePath(string name)
    {
        Directory.CreateDirectory(SaveFolder);  
        return Path.Combine(SaveFolder, name + ".json");
    }
       

    /// <summary>The current game serialised as JSON.</summary>
    public string State => JsonSerializer.Serialize(Snapshot(), SerializerOptions);

    /// <summary>Options used when serialising the game state.</summary>
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// Builds a plain, serialisable snapshot of the whole game: the game type, the
    /// board size, both players' names and every move played, and retains player kinds (Computer or Human) in order.
    /// </summary>
    private GameState Snapshot() => new(
        Type,
        _boards[0].Size,
        PlayerOne.Name,
        PlayerTwo.Name,
        History.ToArray(),// oldest first, so Load replays them in order
        PlayerOne.Kind,
        PlayerTwo.Kind); 

    /// <summary>The serialisable shape of a whole game.</summary>
    private sealed record GameState(
        GameType GameType,
        int BoardSize,
        string PlayerOne,
        string PlayerTwo,
        Placement[] Moves,
        PlayerKind PlayerOneKind,
        PlayerKind PlayerTwoKind);
}
