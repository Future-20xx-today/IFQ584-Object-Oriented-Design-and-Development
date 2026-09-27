namespace TicTacToe;

/// <summary>
/// A participant in a game of Numerical Tic Tac Toe.
/// </summary>
public interface IPlayer
{
    /// <summary>
    /// The player's name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Chooses the next move for this player: which board, and which cell on it.
    /// Any piece or number is decided by the game, not the player. The returned
    /// placement is not guaranteed to be legal; the game validates it.
    /// </summary>
    /// <param name="boards">The game's boards as they currently stand.</param>
    Placement GetMove(IReadOnlyList<IBoard> boards);
}
