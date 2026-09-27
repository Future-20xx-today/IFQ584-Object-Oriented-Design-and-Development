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

    /// <summary>
    /// Prompts this human player for a move at the console.
    /// Expects a "row column" pair, both 0-based, with a board number in front
    /// ("board row column") when the game has more than one board. The player
    /// only chooses where; the game decides what is placed. Keeps asking until
    /// the input parses. Legality is checked by the game, not here.
    /// </summary>
    public override Placement GetMove(IReadOnlyList<IBoard> boards)
    {
        bool chooseBoard = boards.Count > 1;
        string format = chooseBoard ? "board row column" : "row column";
        string example = chooseBoard ? "0 1 2" : "1 2";

        while (true)
        {
            Console.WriteLine($"{Name}'s turn.");
            Console.Write($"Enter your move as \"{format}\": ");
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

            int[] numbers = parts.Select(part => int.TryParse(part, out int n) ? n : -1).ToArray();
            int expected = chooseBoard ? 3 : 2;

            if (numbers.Length == expected && numbers.All(n => n >= 0))
            {
                // The player chooses only where; the game supplies the piece.
                return chooseBoard
                    ? new Placement(numbers[1], numbers[2], 0, numbers[0])
                    : new Placement(numbers[0], numbers[1], 0);
            }

            Console.WriteLine($"Please enter {expected} numbers, e.g. \"{example}\".");
        }
    }
}
