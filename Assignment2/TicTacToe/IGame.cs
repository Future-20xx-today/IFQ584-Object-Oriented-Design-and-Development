namespace TicTacToe;

public interface IGame
{
    /// <summary>
    /// Asks the current player for a move and plays it, re-asking until the move
    /// is legal. Returns what the move meant for the player who made it.
    /// </summary>
    MoveOutcome TakeTurn();

    /// <summary>Draws the current board(s) to the console.</summary>
    void Render();

    /// <summary>Prints how to play this game type.</summary>
    void Help();

    /// <summary>
    /// Persists the current game so it can be reloaded later.
    /// </summary>
    void Save();

    /// <summary>
    /// The current game serialised as JSON: the game type, the two players and
    /// every move played.
    /// </summary>
    string State { get; }

    bool CanUndo { get; }

    bool CanRedo { get; }

    public Placement? Undo();

    public Placement? Redo();

    public IPlayer PlayerOne { get; }

    public IPlayer PlayerTwo { get; }

    public GameType Type { get; }

    public IPlayer CurrentPlayer { get; }
}
