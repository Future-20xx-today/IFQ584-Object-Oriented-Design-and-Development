namespace TicTacToe;

/// <summary>
/// A rectangular grid of cells that pieces are placed on. Holds only what every
/// game variant needs; variant-specific rules (such as the Numerical target sum)
/// belong on the concrete board or the game. A board keeps no move history:
/// the game records moves, and undoes one by removing its piece.
/// </summary>
public interface IBoard
{
    /// <summary>The number of cells along one side of the board.</summary>
    int Size { get; }

    /// <summary>The height of the board in cells.</summary>
    int Height { get; }

    /// <summary>The width of the board in cells.</summary>
    int Width { get; }

    /// <summary>
    /// Gets the piece at the given row and column, or null if the cell is empty.
    /// </summary>
    Piece? GetCell(int row, int column);

    /// <summary>
    /// Places a piece at the given row and column.
    /// Throws if the cell is off the board or already taken.
    /// </summary>
    void PlacePiece(int row, int column, Piece piece);

    /// <summary>
    /// Takes the piece off the given row and column.
    /// Throws if the cell is off the board or already empty.
    /// </summary>
    void RemovePiece(int row, int column);

    /// <summary>Returns true if the given row and column fall within the board.</summary>
    bool IsInBounds(int row, int column);

    /// <summary>True when no empty cells remain.</summary>
    bool IsFull();

    /// <summary>The coordinates of every empty cell, in reading order.</summary>
    IEnumerable<(int Row, int Column)> EmptyCells();
}
