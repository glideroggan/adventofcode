var input = ReadInput("input.txt");

static string[] ReadInput(string filePath)
{
    return File.ReadAllLines(filePath);
}

// as the input is, could it be solved faster by dividing the input into smaller chunks?
// and letting each thread solve a chunk?

var cards = Parse(input);

static Card[] Parse(string[] input)
{
    // parse card number, winning numbers | your numbers
    var cards = new List<Card>();
    foreach (var line in input)
    {
        // var card = new Card();
        var parts = line.Split(":");
        var id = int.Parse(parts[0][parts[0].IndexOf(" ")..]);
        parts = parts[1].Split("|");
        var winningNumbersStr = parts[0].Split(" ", StringSplitOptions.RemoveEmptyEntries);
        var yourNumbersStr = parts[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
        var winningNumbers = winningNumbersStr.Select(x => x.Trim()).Select(int.Parse).ToArray();
        var yourNumbers = yourNumbersStr.Select(x => x.Trim()).Select(int.Parse).ToArray();
        cards.Add(new Card(id, winningNumbers, yourNumbers));
    }
    return cards.ToArray();
}



// calculate the worth of each card
foreach (var card in cards)
{
    card.Worth = card.CalculateWorth();
}

Console.WriteLine($"Sum: {cards.Sum(c => c.Worth)}");

class Card(int Id, int[] WinningNumbers, int[] YourNumbers)
{
    // public required int Id { get; set; }
    // public required int[] WinningNumbers { get; set; }
    // public required int[] YourNumbers { get; set; }
    public int Worth { get; set; }

    internal int CalculateWorth()
    {
        // each matching of your numbers with the winning numbers is worth 1 point
        // every subsequent match is worth double the previous match

        var worth = 0;
        var matches = 0;
        foreach (var yourNumber in YourNumbers)
        {
            if (WinningNumbers.Contains(yourNumber))
            {
                matches++;
                worth = matches == 1 ? 1 : worth * 2;
            }
        }
        return worth;
    }
}