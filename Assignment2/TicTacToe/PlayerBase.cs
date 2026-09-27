namespace TicTacToe;

/// <summary>
/// Base class for the game's players. Holds the state and behaviour common to
/// every player — just a name — so concrete players only need to supply their
/// move-selection strategy in <see cref="GetMove"/>.
///
/// A player only chooses where to move (a board and a cell); what gets placed
/// there is decided by the game.
///
/// This class does not itself declare <see cref="IPlayer"/>; each concrete
/// player implements that interface, using the members provided here to satisfy
/// it.
/// </summary>
public abstract class PlayerBase
{
    /// <summary>
    /// The player's name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initialises the shared player state. Called by subclass constructors.
    /// </summary>
    /// <param name="name">A display name for the player.</param>
    protected PlayerBase(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Chooses this player's next move (a board and a cell) given the game's
    /// boards. Each concrete player (human, computer, ...) provides its own
    /// implementation. The returned move is validated by the game, not here.
    /// </summary>
    public abstract Placement GetMove(IReadOnlyList<IBoard> boards);

    public override string ToString() => Name;
}
