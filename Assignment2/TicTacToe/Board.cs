namespace TicTacToe;

/// <summary>
/// Represents the Numerical Tic Tac Toe playing board.
/// The board is a square grid of size n x n whose cells hold the numbers
/// 1..n^2. It only holds pieces: it keeps no move history (that is the game's
/// job), so a piece goes on with <see cref="PlacePiece"/> and comes off with
/// <see cref="RemovePiece"/>.
/// </summary>
public class Board : IBoard
{
    /// <summary>
    /// The grid of cells. Each cell holds a <see cref="Piece"/>,
    /// or null when the cell is empty.
    /// </summary>
    private readonly Piece?[,] _cells;

    /// <summary>
    /// The size of the board (the number of cells along one side).
    /// Set once via the constructor and cannot be changed afterwards.
    /// </summary>
    public int Size { get; }

    /// <summary>
    /// The height of the board in cells. For a square board this equals Size.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// The width of the board in cells. For a square board this equals Size.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Creates a new square board of the given size. All cells start empty.
    /// </summary>
    /// <param name="n">The number of cells along one side of the board.</param>
    public Board(int n)
    {
        Size = n;
        Height = n;
        Width = n;
        _cells = new Piece?[n, n];
        // A new Piece?[,] defaults every element to null,
        // so the board already starts empty.
    }

    /// <summary>
    /// The number every winning line has to add up to: n(n^2 + 1) / 2, the magic
    /// constant of an n x n square. For the classic 3x3 game this is 15.
    /// </summary>
    public int TargetSum => Size * (Size * Size + 1) / 2;

    /// <summary>
    /// The largest number in play. The numbers 1..HighestNumber are split
    /// between the two players as odds and evens.
    /// </summary>
    public int HighestNumber => Size * Size;

    /// <summary>
    /// Gets the piece at the given row and column, or null if the cell is empty.
    /// </summary>
    public Piece? GetCell(int row, int column)
    {
        if (!IsInBounds(row, column))
        {
            throw new ArgumentOutOfRangeException(
                $"Cell ({row}, {column}) is outside the {Size}x{Size} board.");
        }

        return _cells[row, column];
    }

    /// <summary>
    /// Places a piece at the given row and column. The cell must be on the board
    /// and empty; the game checks that before playing, so breaking it is a bug.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The cell is off the board.</exception>
    /// <exception cref="InvalidOperationException">The cell is already taken.</exception>
    public void PlacePiece(int row, int column, Piece piece)
    {
        if (GetCell(row, column) != null)
        {
            throw new InvalidOperationException($"Cell ({row}, {column}) is already taken.");
        }

        _cells[row, column] = piece;
    }

    /// <summary>
    /// Takes the piece off the given row and column, leaving the cell empty. The
    /// cell must hold a piece; the game only removes a move it played.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The cell is off the board.</exception>
    /// <exception cref="InvalidOperationException">The cell is already empty.</exception>
    public void RemovePiece(int row, int column)
    {
        if (GetCell(row, column) == null)
        {
            throw new InvalidOperationException($"Cell ({row}, {column}) is already empty.");
        }

        _cells[row, column] = null;
    }

    /// <summary>
    /// Returns true if the given row and column fall within the board.
    /// </summary>
    public bool IsInBounds(int row, int column)
    {
        return row >= 0 && row < Height && column >= 0 && column < Width;
    }

    /// <summary>True when no empty cells remain.</summary>
    public bool IsFull()
    {
        for (int row = 0; row < Height; row++)
        {
            for (int column = 0; column < Width; column++)
            {
                if (_cells[row, column] == null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>The coordinates of every empty cell, in reading order.</summary>
    public IEnumerable<(int Row, int Column)> EmptyCells()
    {
        for (int row = 0; row < Height; row++)
        {
            for (int column = 0; column < Width; column++)
            {
                if (_cells[row, column] == null)
                {
                    yield return (row, column);
                }
            }
        }
    }

    public override string ToString()
    {
        // Every mark (X or O) is a single character, so cells are one wide.
        const int cellWidth = 1;
        var builder = new System.Text.StringBuilder();

        for (int row = 0; row < Height; row++)
        {
            for (int column = 0; column < Width; column++)
            {
                // An empty (null) cell renders as a blank; otherwise the piece's
                // ToString() supplies its X or O mark.
                string text = _cells[row, column]?.ToString() ?? string.Empty;

                builder.Append(' ');
                builder.Append(text.PadLeft(cellWidth));
                builder.Append(' ');

                if (column < Width - 1)
                {
                    builder.Append('|');
                }
            }

            builder.AppendLine();

            if (row < Height - 1)
            {
                // Row separator, e.g. "---+---+---" sized to the board width.
                builder.AppendLine(string.Join(
                    "+", Enumerable.Repeat(new string('-', cellWidth + 2), Width)));
            }
        }

        return builder.ToString();
    }
}
