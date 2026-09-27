namespace TicTacToe;

/// <summary>
/// Notakto: both players play X on three 3x3 boards. A board with three in a
/// row is dead and can't be played on; whoever kills the last board loses.
/// </summary>
public sealed class NotaktoGame : Game, IGame
{
    public NotaktoGame(IPlayer playerOne, IPlayer playerTwo)
        : base(playerOne, playerTwo, new Board(3), new Board(3), new Board(3))
    {
    }

    public override GameType Type => GameType.Notakto;

    public bool IsBoardDead(int index)
    {
        throw new NotImplementedException();
    }

    private bool IsDead(Board board)
    {
        throw new NotImplementedException();
    }

    protected override bool IsLegal(Placement placement)
    {
        throw new NotImplementedException();
    }

    protected override MoveOutcome PlayMove(Placement placement)
    {
        throw new NotImplementedException();
    }

    public void Help()
    {
        Console.WriteLine("Both players play X on three 3x3 boards (numbered 0 to 2).");
        Console.WriteLine("A board with three in a row is dead and can't be played on.");
        Console.WriteLine("Whoever kills the last board loses.");
        Console.WriteLine("Enter moves as \"board row column\", e.g. \"0 1 2\".");
        Console.WriteLine();
    }

    private static bool IsMatch(Board board, (int Row, int Column)[] line)
    {
        throw new NotImplementedException();
    }
}
