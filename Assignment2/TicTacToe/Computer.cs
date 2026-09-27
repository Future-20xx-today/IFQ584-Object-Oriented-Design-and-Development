namespace TicTacToe;

/// <summary>
/// An <see cref="IPlayer"/> controlled by the computer.
///
/// The strategy is deliberately simple: if playing the next number in any empty
/// cell would win the game on the spot, it plays that cell. Otherwise it plays
/// the number in a randomly chosen empty cell.
/// </summary>
public class Computer : PlayerBase, IPlayer
{
    /// <summary>
    /// Creates a new computer player.
    /// </summary>
    /// <param name="name">A display name for the player. Defaults to "Computer".</param>
    public Computer(string name = "Computer")
        : base(name)
    {
    }

    /// <summary>
    /// Chooses the computer's next move: an immediately winning cell if it can
    /// find one, otherwise a random empty cell on a random board that has room.
    ///
    /// Assumes the game is not already over — that there is at least one empty
    /// cell — which the game loop checks before asking for a move.
    /// </summary>
    public override Placement GetMove(IReadOnlyList<IBoard> boards)
    {
        // Only a board with an empty cell can take a move.
        int[] open = Enumerable.Range(0, boards.Count).Where(i => !boards[i].IsFull()).ToArray();
        int boardIndex = open[Random.Shared.Next(open.Length)];
        var cells = boards[boardIndex].EmptyCells().ToList();

        // TODO: Board.IsWinningMove no longer exists - win check moves to GameVariant //
        // foreach ((int row, int column) in cells)
        // {
        //     if (board.IsWinningMove(row, column, number))
        //     {
        //         return new Move(row, column, number);
        //     }
        // }

        // No winning cell, so play in a random empty cell. The game supplies the piece.
        (int Row, int Column) cell = cells[Random.Shared.Next(cells.Count)];

        return new Placement(cell.Row, cell.Column, 0, boardIndex);
    }
}
