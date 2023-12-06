var inputData = File.ReadAllLines("input.txt");

var races = Parse(inputData);



/* for each race
come up with the range from starting ms, to ending ms that holding the button
would win.
The charge is 1mm/ms
*/
var CalculateDistance = (double holdingMs, double totalMs) =>
    {
        return holdingMs * (totalMs - holdingMs);
    };
var CalculateHolding = (double distance_mm, double totalMs) =>
    {
        return (max: (double)((totalMs / 2d) + (Math.Sqrt(-4f * distance_mm + totalMs * totalMs) / 2d)),
            min: (double)((totalMs / 2d) - (Math.Sqrt(-4f * distance_mm + totalMs * totalMs) / 2d)));

    };
var product = 1;
foreach (var race in races)
{
    // need to come up with the forumla for the lowest "holding" ms
    // can we come up with the holding ms for the record distance?
    // should just be in reverse
    var holdingMs = CalculateHolding(race.MaxDistance, race.Time);
    // Console.WriteLine($"Record holder held {holdingMs}ms to get a distance of {race.MaxDistance}mm");
    var minimumHoldingForWin = Math.Ceiling(holdingMs.min) == holdingMs.min
        ? holdingMs.min + 1
        : Math.Ceiling(holdingMs.min);
    var distance_mm = CalculateDistance(minimumHoldingForWin, race.Time);
    // Console.WriteLine($"To get a distance of {distance_mm}mm, hold for {minimumHoldingForWin}ms");

    var maximumHoldingForWin = Math.Floor(holdingMs.max) == holdingMs.max
        ? holdingMs.max - 1
        : Math.Floor(holdingMs.max);
    distance_mm = CalculateDistance(maximumHoldingForWin, race.Time);
    // Console.WriteLine($"To get a distance of {distance_mm}mm, hold for {maximumHoldingForWin}ms");

    var numbersOfHoldStepsToWin = (int)(maximumHoldingForWin - minimumHoldingForWin + 1);
    // Console.WriteLine($"Steps: {numbersOfHoldStepsToWin}");
    product *= numbersOfHoldStepsToWin;
}

Console.WriteLine($"Product of all the possible holding times is {product}");

static Race[] Parse(string[] inputData)
{
    var races = new List<Race>();
    var times = inputData[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1)
        .Aggregate("", (acc, next) => acc + next);
    var distances = inputData[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1)
        .Aggregate("", (acc, next) => acc + next);
    Console.WriteLine("""
    Times: {0}
    Distances: {1}
    """, times, distances);
    races.Add(new Race(ulong.Parse(times), ulong.Parse(distances)));
    // for (var i = 0; i < times.Length; i++)
    // {
    //     races.Add(new Race(times[i], distances[i]));
    // }
    return races.ToArray();
}

record struct Race(ulong Time, ulong MaxDistance);