using System.Security.Cryptography.X509Certificates;

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
    var copies = new Dictionary<int, int>();
    var cards = new List<Card>();

    for (var index = 0; index < input.Length; index++)
    {
        var line = input[index];

        // process original card
        var parts = line.Split(":");
        var id = int.Parse(parts[0][parts[0].IndexOf(' ')..]);
        parts = parts[1].Split("|");
        var winningNumbersStr = parts[0].Split(" ", StringSplitOptions.RemoveEmptyEntries);
        var yourNumbersStr = parts[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
        var winningNumbers = winningNumbersStr.Select(x => x.Trim()).Select(int.Parse).ToArray();
        var yourNumbers = yourNumbersStr.Select(x => x.Trim()).Select(int.Parse).ToArray();
        var card = new Card(id, winningNumbers, yourNumbers);
        cards.Add(card);

        // add copies based on matches of winning numbers and your numbers
        var matches = winningNumbers.Where(winningNumber => yourNumbers.Contains(winningNumber)).ToList();
        // for each match, add a copy of the following card
        var copiesId = id + 1;
        for (var i = 0; i < matches.Count; i++)
        {
            if (copies.TryGetValue(copiesId, out var copiesCount))
            {
                copies[copiesId] = copiesCount + 1;
            }
            else
            {
                copies.Add(copiesId, 1);
            }

            copiesId++;
        }

        // for current card, check if there are copies that needs processing
        if (copies.TryGetValue(id, out var num))
        {
            // add copies to current card
            card.Copies = num;
        }
        
        // if card had copies, process those copies to get copies
        for (var i = 0; i < card.Copies; i++)
        {
            // get matches
            matches = card.WinningNumbers.Where(winningNumber => card.YourNumbers.Contains(winningNumber)).ToList();

            copiesId = card.Id + 1;
            for (var i2 = 0; i2 < matches.Count; i2++)
            {
                if (copies.TryGetValue(copiesId, out var copiesCount))
                {
                    copies[copiesId] = copiesCount + 1;
                }
                else
                {
                    copies.Add(copiesId, 1);
                }

                copiesId++;
            }
        }
    }

    return cards.ToArray();
}


// sum up the number of cards, including copies
var totalCards = cards.Sum(x => x.Copies + 1);
Console.WriteLine($"Total cards: {totalCards}");

internal class Card(int id, int[] winningNumbers, int[] yourNumbers)
{
    public int[] WinningNumbers { get; } = winningNumbers;
    public int[] YourNumbers { get; } = yourNumbers;
    public int Copies { get; set; }
    public int Id { get; } = id;
}