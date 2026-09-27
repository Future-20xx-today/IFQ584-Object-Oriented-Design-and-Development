namespace TicTacToe;

/// Information to support player moves and computer 'check if wins' logic, without keeping ///
/// 

public interface IGameView
{
    /// All valid moves for the specific player //
    IReadOnlyList<Placement> CandidateMoves();

    /// What playing the move would mean for Win or lose, but without committing it///
    MoveOutcome Preview(Placement p);

}
