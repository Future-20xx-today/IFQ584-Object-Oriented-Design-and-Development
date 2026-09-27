namespace TicTacToe;

/// <summary>
/// Gomoku: players take turns placing their own stones on the intersections of
/// a large grid; the first to get <see cref="InRow"/> of their stones in an
/// unbroken line (across, down or diagonal) wins. Five or more counts
/// (freestyle Gomoku).
/// </summary>
public sealed class GomokuGame : Game, IGame
{
    public const int InRow = 5;

    public GomokuGame(IPlayer playerOne, IPlayer playerTwo)
        : base(playerOne, playerTwo, new GomokuBoard())
    {
    }

    public override GameType Type => GameType.Gomoku;

    /// <summary>Gomoku is played on a single board.</summary>
    private IBoard Grid => Boards[0];

    /// <summary>
    /// The stone for the move being placed: 1 (X) for player one, 2 (O) for
    /// player two.
    /// </summary>
    private int CurrentPiece => MoveCount % 2 == 0 ? 1 : 2;

    /// <summary>
    /// Gomoku adds no rule beyond the point being on the board and empty, which
    /// <see cref="Game.Play"/> has already checked.
    /// </summary>
    protected override bool IsLegal(Placement placement) => true;

    /// <summary>
    /// Places the current player's stone, then checks whether it completed a
    /// line of <see cref="InRow"/> or filled the board.
    /// </summary>
    protected override MoveOutcome PlayMove(Placement placement)
    {
        Piece stone = new(CurrentPiece);
        Grid.PlacePiece(placement.Row, placement.Column, stone);

        if (Lines(Grid, InRow).Any(line => IsMatch(Grid, line, stone.Mark)))
        {
            return MoveOutcome.CurrentPlayerWins;
        }

        return Grid.IsFull() ? MoveOutcome.Draw : MoveOutcome.Continue;
    }

    public void Help()
    {
        Console.WriteLine();
        Console.WriteLine("How to play Gomoku");
        Console.WriteLine("------------------");
        Console.WriteLine($"The board is a grid of {Grid.Size} x {Grid.Size} lines. Stones go on");
        Console.WriteLine("the points where the lines cross ('+'), not in the squares.");
        Console.WriteLine($"{PlayerOne} plays X and moves first; {PlayerTwo} plays O.");
        Console.WriteLine();
        Console.WriteLine($"Take turns placing a stone on an empty point. The first to get {InRow}");
        Console.WriteLine("or more of their own stones in an unbroken line, across, down or");
        Console.WriteLine("diagonally, wins. If the board fills first, it's a draw.");
        Console.WriteLine();
    }

    /// <summary>True if every point in the line holds a stone with the given mark.</summary>
    private static bool IsMatch(IBoard board, (int Row, int Column)[] line, char mark)
    {
        foreach ((int row, int column) in line)
        {
            if (board.GetCell(row, column)?.Mark != mark)
            {
                return false;
            }
        }

        return true;
    }
}
