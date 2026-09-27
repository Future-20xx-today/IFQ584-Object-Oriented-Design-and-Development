namespace TicTacToe;

/// <summary>
/// A human player that chooses its moves by typing them at the console.
/// </summary>
public class Player : PlayerBase, IPlayer
{
    /// <summary>
    /// Creates a new human player with the given name.
    /// </summary>
    public Player(string name)
        : base(name)
    {
    }

    public PlayerKind Kind => PlayerKind.Human; // Sets player as human for save / load logic

    /// <summary>
    /// Prompts this human player for a move at the console.
    /// Expects a "row column" pair, both 0-based; the number to place is the
    /// game's next number, so the player only chooses the cell. Keeps asking
    /// until the input parses. Legality against the board is checked by the game
    /// loop, not here.
    /// </summary>
    public override Move GetMove(Board board)
    {
        while (true)
        {
            //Console.WriteLine($"{Name}'s turn. You play the number ///{board.NextNumber}///."); // blanked out as this is just runing through numbers in order
            Console.Write("Enter your move as \"row column\": ");
            string? line = Console.ReadLine();

            // Console.ReadLine returns null at end of input (e.g. Ctrl-D, or a
            // closed/redirected stream). There is no more input to read, so
            // looping would spin forever; end the program cleanly instead.
            if (line == null)
            {
                Console.WriteLine();
                Console.WriteLine("No more input. Goodbye.");
                Environment.Exit(0);
            }

            var parts = line.Split(
                new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts is { Length: 2 } &&
                int.TryParse(parts[0], out int row) &&
                int.TryParse(parts[1], out int column))
            {
                // The player chooses only the cell; the board's next number is
                // what gets placed.
                return new Move(row, column, board.NextNumber);
            }

            Console.WriteLine("Please enter two numbers, e.g. \"1 2\".");
        }
    }

    public Placement ChoosePlacement(IGameView game, Move cell)
    {
        var options = game.CandidateMoves()
            .Where(p => p.Row == cell.Row && p.Column == cell.Column)
            .ToList();

        if (options.Count == 1)
            return options[0]; /// gives number choice for games where this exists only

        var numbers = options.Select(p => p.SelectedNumber).ToList();
        while (true)
        {
            Console.Write($"Choose a number ({string.Join(", ", numbers)}): ");
            string? line = Console.ReadLine();

            if (line == null)
            {
                Console.WriteLine();
                Console.WriteLine("No more input. Goodbye.");
                Environment.Exit(0);
            }

            if (int.TryParse(line, out int n) && numbers.Contains(n))
                return options.First(p => p.SelectedNumber == n);

            Console.WriteLine("That number isn't available, try again.");
        }
    }
}
