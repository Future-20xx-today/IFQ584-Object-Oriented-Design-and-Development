using System.Data.Common;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TicTacToe;

/// <summary>
/// Numerical Tic Tac Toe on an n x n board. The players share the numbers
/// 1..n^2 and play them in order; whoever completes a line of n numbers adding
/// up to the board's target sum wins, no matter who played the other numbers.
/// </summary>
public sealed class NumericalTTTGame : Game, IGame
{

    /// <summary>
    /// Base for NumericalTTTGame
    /// </summary>
    /// <param name="playerOne"></param>
    /// <param name="playerTwo"></param>
    /// <param name="Board"></param>
    /// <param name=""></param>
    public NumericalTTTGame(IPlayer playerOne, IPlayer playerTwo, int Board.size) ///correct for variable input for no at game start
        : base(playerOne, playerTwo, new Board(size))
    {
    }

    public override GameType Type => GameType.NumericalTicTacToe;

    public bool IsLegal(Player.GetMove row, col, num)///check against inputs, passing Placement as argument or both values?
    {
        if (board[row, col] == 0 || board[row, col] == null)
        {
            return true;
        }
        else
        {
            Console.WriteLine($"Square must be empty. Please try again.\n"); ///is a writeline needed here or just the return and writeline elsewhere
            return false;
        }
    }
    public MoveOutcome PlayMove(Player.GetMove row, col, num)///passing Player.GetMove as argument or all 3 values, what was this changed to? PlayMove or MoveOutCome
    {
        if IsLegal
        {
            board[row, column] = num;
            Console.WriteLine($"Previous player move placed {number} onto the board");/// standard elsewhere or needed here?
            Console.WriteLine("Press any key to continue\n"); Console.ReadKey();///As above
            ICommand command = _undo.Push(row, col, num);///change it in Game class from private readonly to allow access for one source of truth that can be done, undone, saved and reloaded.
        }
        else { Console.WriteLine($"Unknown error after validation.\n"); }

    /// Assume below was removed as we worked through before.
/*public void Apply(Placement p)
{
    Board board = Boards[p.BoardIndex];
    board.PlacePiece(p.Row, p.Column, new Piece(board.NextNumber));
}
*/
/// Was removed?
/*public MoveOutcome Evaluate(Placement p)
{
    Board board = Boards[p.BoardIndex];

    if (Lines(board, board.Size).Any(line => IsMatch(board, line)))
    {
        return MoveOutcome.CurrentPlayerWins;
    }

    The numbers run out exactly when the board fills.
    return board.IsFull() ? MoveOutcome.Draw : MoveOutcome.Continue;
}*/

/// <summary>True if every cell in the line is filled and they add up to the target sum.</summary>
/*passing to a standard checkwin as same calc for TTT as Notakto
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
}*/

