using System.Text.Json;

var dataInput = ReadInput("input.txt");

var games = Parse(dataInput);

// Console.WriteLine($"Games: {JsonSerializer.Serialize(games)}");

const string cards = "AKQT98765432J";

static Hand[] SortHands(Hand[] hands)
{
    // lets sort this by having an array that are the indexes into the source array
    // so we move through the array and compare the cards
    // or maybe we also needs buckets for kinds, so we individually sort the buckets afterwards
    var kindBuckets = new Dictionary<Kind, List<Hand>>();

    // determine kind for each hand and place in bucket
    foreach (var hand in hands)
    {
        // record values
        var cardBucket = new Dictionary<char, int>();
        var jokers = 0;
        foreach (var card in hand.Cards)
        {
            if (card == 'J')
            {
                jokers++;
            }
            else
            {
                cardBucket[card] = cardBucket.TryGetValue(card, out var count) ? count + 1 : 1;
            }
        }

        var kind = GetKind(cardBucket, jokers);
        (kindBuckets.TryGetValue(kind, out var bucket)
            ? bucket
            : kindBuckets[kind] = new List<Hand>()).Add(hand);
    }

    // Console.WriteLine($"Buckets: {JsonSerializer.Serialize(kindBuckets)}");
    var valueSorter = Comparer<Hand>.Create((a, b) =>
    {
        for (var i = 0; i < a.Cards.Length; i++)
        {
            var aCard = a.Cards[i];
            var bCard = b.Cards[i];
            if (aCard == bCard) continue;
            var aIndex = cards.IndexOf(aCard);
            var bIndex = cards.IndexOf(bCard);
            return aIndex < bIndex ? 1 : -1;
        }

        return 0;
    });
    // now that we have determined the kind, we can sort each bucket that have more than one hand
    foreach (var bucket in kindBuckets.Values)
    {
        if (bucket.Count > 1)
        {
            bucket.Sort(valueSorter);
        }
    }

    // return a new array with the correct sort order
    var sorted = new List<Hand>();
    foreach (var kind in Enum.GetValues<Kind>())
    {
        if (kindBuckets.TryGetValue(kind, out var bucket))
        {
            sorted.AddRange(bucket);
        }
    }

    return sorted.ToArray();
}

// sort the hands
var hands = games.Select(g => new Hand(g.Cards, 0, g.Bid)).ToList();
// Console.WriteLine($"Hands: {JsonSerializer.Serialize(hands)}");
// can't sort with the Comparer, as it can't really distinguish between the two different ways of sorting
// hands.Sort(HandSorter);
hands = SortHands(hands.ToArray()).ToList();
// set rank after order
for (var i = hands.Count - 1; i >= 0; i--)
{
    hands[i] = hands[i] with { Rank = i + 1 };
}

// Console.WriteLine($"Hands: {JsonSerializer.Serialize(hands)}");

// calculate the score, by multiplying the rank with the bid
var score = hands.Sum(h => h.Rank * h.Bid);
Console.WriteLine($"Score: {score}");
return;

static List<Game> Parse(string[] data)
{
    var games = new List<Game>();
    foreach (var line in data)
    {
        var parts = line.Split(" ");
        var cards = parts[0];
        var bid = int.Parse(parts[^1]);
        games.Add(new Game(cards, bid));
    }

    return games;
}

static string[] ReadInput(string file) => File.ReadAllLines(file);

static Kind GetKind(Dictionary<char, int> cards, int jokers)
{
    // lets find the highest Kind, and using jokers to fill in the gaps
    if (jokers == 0)
    {
        return cards.Values switch
        {
            var values when values.Any(v => v == 5) => Kind.FiveOfAKind,
            var values when values.Any(v => v == 4) => Kind.FourOfAKind,
            var values when values.Any(v => v == 3) && values.Any(v => v == 2) => Kind.FullHouse,
            var values when values.Any(v => v == 3) => Kind.ThreeOfAKind,
            var values when values.Count(v => v == 2) == 2 => Kind.TwoPairs,
            var values when values.Any(v => v == 2) => Kind.OnePair,
            _ => Kind.HighCard,
        };
    }

    // five of a kind is the highest
    // four of a kind => five of a kind
    // Full house => not possible
    // three of a kind => four of a kind
    // two pairs => three of a kind
    // one pair => three of a kind 

    // try to find the highest possible kind
    var kind = cards.Values switch
    {
        var values when values.Any(v => v == 5) => Kind.FiveOfAKind,
        var values when values.Any(v => v == 4) => Kind.FourOfAKind,
        var values when values.Any(v => v == 3) && values.Any(v => v == 2) => Kind.FullHouse,
        var values when values.Any(v => v == 3) => Kind.ThreeOfAKind,
        var values when values.Count(v => v == 2) == 2 => Kind.TwoPairs,
        var values when values.Any(v => v == 2) => Kind.OnePair,
        _ => Kind.HighCard,
    };
    while (jokers > 0 && kind != Kind.FiveOfAKind)
    {
        kind = kind switch
        {
            Kind.HighCard => Kind.OnePair,
            Kind.OnePair => Kind.ThreeOfAKind,
            Kind.TwoPairs => Kind.FullHouse,
            Kind.ThreeOfAKind => Kind.FourOfAKind,
            Kind.FullHouse => Kind.FourOfAKind,
            Kind.FourOfAKind => Kind.FiveOfAKind,
            _ => throw new Exception("Invalid kind"),
        };
        jokers--;
    }
    return kind;
}

internal enum Kind
{
    FiveOfAKind = 10,
    FourOfAKind = 9,
    FullHouse = 8,
    ThreeOfAKind = 7,
    TwoPairs = 6,
    OnePair = 5,
    HighCard = 4
}

internal record struct Hand(string Cards, int Rank, int Bid);

internal record struct Game(string Cards, int Bid);