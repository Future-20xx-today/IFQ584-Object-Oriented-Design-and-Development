namespace TicTacToe;

/// <summary>
/// The Gomoku playing board: a grid of lines, like a Go board. Stones are
/// placed on the intersections where the lines cross, not inside squares, so a
/// board of n lines each way has n x n playable points.
///
/// Through <see cref="IBoard"/> each intersection is addressed as a "cell" by
/// row and column; here a row or column means one of the grid lines. The board
/// only holds stones and keeps no move history; that is the game's job.
/// </summary>
public class GomokuBoard : IBoard
{
    /// <summary>
    /// The stone on each intersection, or null when the point is empty.
    /// </summary>
    private readonly Piece?[,] _points;

    /// <summary>The number of grid lines each way (15 for a standard board).</summary>
    public int Size { get { return 15; } }

    /// <summary>The number of horizontal lines. Equals Size.</summary>
    public int Height => Size;

    /// <summary>The number of vertical lines. Equals Size.</summary>
    public int Width => Size;

    /// <summary>
    /// Creates an empty board with the 15 lines each way.
    /// </summary>
    public GomokuBoard()
    {
        _points = new Piece?[this.Size, this.Size];
    }

    /// <summary>
    /// Gets the stone on the intersection of the given lines, or null if it is
    /// empty.
    /// </summary>
    public Piece? GetCell(int row, int column)
    {
        if (!IsInBounds(row, column))
        {
            throw new ArgumentOutOfRangeException(
                $"Point ({row}, {column}) is off the {Size}x{Size} board.");
        }

        return _points[row, column];
    }

    /// <summary>
    /// Places a stone on the intersection of the given lines. The point must be
    /// on the board and empty; the game checks that before playing, so breaking
    /// it is a bug.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The point is off the board.</exception>
    /// <exception cref="InvalidOperationException">The point is already taken.</exception>
    public void PlacePiece(int row, int column, Piece piece)
    {
        if (GetCell(row, column) != null)
        {
            throw new InvalidOperationException($"Point ({row}, {column}) is already taken.");
        }

        _points[row, column] = piece;
    }

    /// <summary>
    /// Takes the stone off the intersection of the given lines. The point must
    /// hold a stone; the game only removes a move it played.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The point is off the board.</exception>
    /// <exception cref="InvalidOperationException">The point is already empty.</exception>
    public void RemovePiece(int row, int column)
    {
        if (GetCell(row, column) == null)
        {
            throw new InvalidOperationException($"Point ({row}, {column}) is already empty.");
        }

        _points[row, column] = null;
    }

    /// <summary>Returns true if the given lines cross on this board.</summary>
    public bool IsInBounds(int row, int column)
    {
        return row >= 0 && row < Size && column >= 0 && column < Size;
    }

    /// <summary>True when every intersection holds a stone.</summary>
    public bool IsFull() => !EmptyCells().Any();

    /// <summary>The coordinates of every empty intersection, in reading order.</summary>
    public IEnumerable<(int Row, int Column)> EmptyCells()
    {
        for (int row = 0; row < Size; row++)
        {
            for (int column = 0; column < Size; column++)
            {
                if (_points[row, column] == null)
                {
                    yield return (row, column);
                }
            }
        }
    }

    /// <summary>
    /// Draws the grid with stones on the intersections, e.g.
    /// <code>
    ///     0  1  2
    ///  0  +--X--+
    ///  1  +--O--+
    /// </code>
    /// An empty point is '+', and '-' runs along each horizontal line.
    /// </summary>
    public override string ToString()
    {
        var builder = new System.Text.StringBuilder();

        // Column numbers, each above its vertical line.
        builder.Append("    ");
        for (int column = 0; column < Size; column++)
        {
            builder.Append(column.ToString().PadRight(3));
        }
        builder.AppendLine();

        for (int row = 0; row < Size; row++)
        {
            builder.Append(row.ToString().PadLeft(2)).Append("  ");

            for (int column = 0; column < Size; column++)
            {
                builder.Append(_points[row, column]?.ToString() ?? "+");

                if (column < Size - 1)
                {
                    builder.Append("--");
                }
            }

            builder.AppendLine();
        }

        return builder.ToString();
    }
}
