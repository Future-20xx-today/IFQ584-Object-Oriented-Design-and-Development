namespace TicTacToe;

/// <summary>
/// An <see cref="IPlayer"/> controlled by the computer.
///
/// The strategy is deliberately simple: if a winning move is available it plays that move. Otherwise it plays
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

    public PlayerKind Kind => PlayerKind.Computer; // Sets player as computer for save / load logic

    /// <summary>
    /// Chooses the computer's next move: an immediately winning cell if it can
    /// find one, otherwise a random empty cell.
    ///
    /// Assumes the game is not already over — that there is at least one empty
    /// cell — which the game loop checks before asking for a move.
    /// </summary>
    public override Move GetMove(Board board)
    {
        var cells = board.EmptyCells().ToList();

        // provides next number (for games where this is needed //
        int number = board.NextNumber;

        // No winning cell, so play the number in a random empty cell.
        (int Row, int Column) cell = cells[Random.Shared.Next(cells.Count)];

        return new Move(cell.Row, cell.Column, number);
    }

    // Computer win checker and taker, if available. //
    public Placement ChoosePlacement(IGameView game, Move cell)
    {
        IReadOnlyList<Placement> options = game.CandidateMoves();

        foreach (Placement p in options)
        {
            if (game.Preview(p) == MoveOutcome.CurrentPlayerWins)
                return p; // computer take the win if this is availabe
        }

        return options[Random.Shared.Next(options.Count)]; // or else play random move

    }

}