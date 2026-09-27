
namespace TicTacToe;

/// <summary>
/// Numerical Tic Tac Toe on an n x n board. The players share the numbers
/// 1..n^2 ; whoever completes a line of n numbers adding
/// up to the board's target sum wins, no matter who played the other numbers.
/// </summary>
public sealed class NumericalTTTGame : Game, IGame
{
    public NumberLists Numbers { get; }
    public NumericalTTTGame(IPlayer playerOne, IPlayer playerTwo, int size = 3)
        : base(playerOne, playerTwo, new Board(size))
    {
        Numbers = new NumberLists(Boards[0].Size, playerOne);
    }



    public override GameType Type => GameType.NumericalTicTacToe;


    /// <summary>
    /// Provides every legal move (vacant square and available value) a player can make
    /// </summary>
    /// <returns></returns>
    public override IReadOnlyList<Placement> CandidateMoves()

    {
        Board board = Boards[0];
        List<int> numbers = Numbers.GetPlayerList(CurrentPlayer);

        return board.EmptyCells()
            .SelectMany(c => numbers.Select(n => new Placement(c.Row, c.Column, n)))
            .ToList();
    }

    /// <summary>
    /// Determines whether the given placement is a legal move for the current player.
    /// </summary>
    /// <param name="p">The placement describing the board, cell, and number to play.</param>
    /// <returns>
    /// <c>true</c> if the target cell is empty and the selected number is available
    /// to the current player; otherwise, <c>false</c>.
    /// </returns>
    public bool IsLegal(Placement p)
    {
        Board board = Boards[p.BoardIndex];
        return board.GetCell(p.Row, p.Column) is null
            && Numbers.GetPlayerList(CurrentPlayer).Contains(p.SelectedNumber); // true ( legal)  only if the cell is empty

    }

    public MoveOutcome PlayMove(Placement p) => Evaluate(p) ;


    /// <summary>
    /// Applies the given placement to the game by placing a <see cref="NumberPiece"/>
    /// on the target cell and marking the selected number as used by the current player.
    /// </summary>
    /// <param name="p">The placement describing the board, cell, and number to play.</param>
    /// <remarks>
    /// This method does not check legality. Call <see cref="IsLegal(Placement)"/>
    /// first to make sure the cell is empty and the number is available.
    /// </remarks>
    public void Apply(Placement p)
    {
        Board board = Boards[p.BoardIndex];
        board.PlacePiece(p.Row, p.Column, new NumberPiece(p.SelectedNumber));
        Numbers.UsedNumbers(CurrentPlayer, p.SelectedNumber);
    }

    protected override void Unapply(Placement p) =>
    Numbers.ReturnNumber(CurrentPlayer, p.SelectedNumber);

    public MoveOutcome Evaluate(Placement p)
    {
        Board board = Boards[p.BoardIndex];

        if (Lines(board, board.Size).Any(line => IsMatch(board, line)))
        {
            return MoveOutcome.CurrentPlayerWins;
        }

        // The numbers run out exactly when the board fills.
        return board.IsFull() ? MoveOutcome.Draw : MoveOutcome.Continue;
    }

    /// <summary>True if every cell in the line is filled and they add up to the target sum.</summary>
    private static bool IsMatch(Board board, (int Row, int Column)[] line)
    {
        int sum = 0;

        foreach ((int row, int column) in line)
        {
            Piece? piece = board.GetCell(row, column);
            if (piece is null)
            {
                return false;
            }

            sum += piece.Value;
        }

        return sum == board.TargetSum;
    }

}

public class NumberLists // Number Lists Class //
{
    private readonly IPlayer _playerOne;
    public List<int> PlayerOneList { get; } //player one list (evens)
    public List<int> PlayerTwoList { get; } //player two list (odds)
    public List<int> GetPlayerList(IPlayer player) => player == _playerOne ? PlayerOneList : PlayerTwoList;
    public NumberLists(int boardSize, IPlayer playerOne)
    {
        _playerOne = playerOne;

        PlayerOneList = new List<int>(); //list constructors//
        PlayerTwoList = new List<int>();
        for (int i = 1; i <= (boardSize * boardSize); i++)
        {
            if (i % 2 == 0)
                PlayerTwoList.Add(i); // Adds evens to PlayerTwoList
            else
                PlayerOneList.Add(i); // Adds odds to PlayerOneList
        }
    }
    public bool UsedNumbers(IPlayer player, int number) // Method to check and remove the number from numlist //
    {
        List<int> list = GetPlayerList(player);
        if (list.Contains(number))
        {
            list.Remove(number);
            return true;  // Able to use this number, and remove
        }
        else return false; // Not able to use this number ( already used)
    }

    public void ReturnNumber(IPlayer player, int number)
    {
        List<int> list = GetPlayerList(player);
        if(!list.Contains(number))
        {
            list.Add(number);
            list.Sort();
        }
    }


    void ShowHelp()
    {
        Console.WriteLine();
        // Console.WriteLine("How to play Numerical Tic Tac Toe");
        // Console.WriteLine("---------------------------------");
        // Console.WriteLine("The board is an n x n grid played with the numbers 1 to n^2.");
        // Console.WriteLine("Player One has Odd numbers, Player 2 has even Numbers. Player 1 moves first.");
        // Console.WriteLine();
        // Console.WriteLine("Players take turns placing available numbers in an empty cell. The");
        // Console.WriteLine("first to complete a row, column or diagonal whose numbers add up");
        // Console.WriteLine("to n(n^2 + 1) / 2 (15 on a 3x3 board) wins — no matter who played");
        // Console.WriteLine("the other numbers in that line.");
        // Console.WriteLine();
        // Console.WriteLine("On your turn:");

    }
}

