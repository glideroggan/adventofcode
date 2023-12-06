
using System.Runtime.CompilerServices;

var dataLines = File.ReadAllLines("input.txt");

var seedRanges = ParseSeeds(dataLines[0]);

static List<(uint Start, uint Length)> ParseSeeds(string line)
{
    var ret = new List<(uint Start, uint Length)>();
    var parts = line.Split(':');
    var numbers = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var s = numbers.Select(s => s.Trim()).Select(s => uint.Parse(s)).ToArray();
    for (var i = 0; i < s.Length; i += 2)
    {
        ret.Add((s[i], s[i + 1]));
    }

    return ret;
}

var maps = ParseMaps(dataLines);

static Maps ParseMaps(string[] dataLines)
{
    // parse out all the maps
    // each map starts with "map:" and ends with an empty line
    var maps = new Maps();
    var mappings = new Map[] {
        maps.Seed2Soil,
        maps.Soil2Fertilizer,
        maps.Fertilizer2Water,
        maps.Water2Light,
        maps.Light2Temperature,
        maps.Temperature2Humidity,
        maps.Humidity2Location
    };
    var mappingIndex = 0;
    for (var i = 2; i < dataLines.Length; i++)
    {
        var line = dataLines[i];
        if (line == string.Empty)
        {
            mappingIndex++;
            continue;
        }

        if (line.EndsWith(":"))
        {
            continue;
        }
        var numbers = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var sourceStart = uint.Parse(numbers[1]);
        var destinationStart = uint.Parse(numbers[0]);
        var length = uint.Parse(numbers[2]);
        var map = mappings[mappingIndex];

        map.AddMapping(sourceStart, destinationStart, length);
    }

    return maps;
}

// get each location for each seed
var lowestLocation = uint.MaxValue;
// How much does the seed ranges overlap? none!
seedRanges.Sort((a, b) => a.Start.CompareTo(b.Start));

// GetOverlaps(seedRanges);

// return;

var tasks = new List<Task>();
var totalRuns = 0UL;
var runs = 0UL;
// for (var index = 0; index < seedRanges.Count; index++)
foreach (var currentSeed in seedRanges)
{
    tasks.Add(MapToLocation(currentSeed.Start, currentSeed.Length));
    Console.WriteLine($"Created Task range: {currentSeed.Start} - {currentSeed.Start + currentSeed.Length}");
}

// report progress, by waiting 5s and then checking if there are any tasks left
while (tasks.Any())
{
    await Task.Delay(5000);
    totalRuns += runs;
    Console.WriteLine($"Tasks left: {tasks.Count,2} Runs: {totalRuns,15}, speed: {runs / 5 / 1_000_000}M runs/s");
    runs = 0;
    tasks.RemoveAll(t => t.IsCompleted);
}


// lowest location
Console.WriteLine($"Lowest location: {lowestLocation}");

Task MapToLocation(uint currentSeed, uint range)
{
    return Task.Factory.StartNew(() =>
    {
        for (var seed = currentSeed; seed < currentSeed + range; seed++)
        {
            var soil = maps.Seed2Soil.Get(seed);
            var fertilizer = maps.Soil2Fertilizer.Get(soil);
            var water = maps.Fertilizer2Water.Get(fertilizer);
            var light = maps.Water2Light.Get(water);
            var temperature = maps.Light2Temperature.Get(light);
            var humidity = maps.Temperature2Humidity.Get(temperature);
            var location = maps.Humidity2Location.Get(humidity);
            // TODO: possible for race conditions
            if (location <= lowestLocation)
            {
                lowestLocation = location;
            }
            runs++;
        }
    });
}
void GetOverlaps(List<(uint Start, uint Length)> seedRanges)
{
    var numbers = new List<(uint Start, uint Length)>();
    var index = 1;
    foreach (var seed in seedRanges)
    {
        var start = seed.Start;
        Console.WriteLine($"{start}");
        var end = start + seed.Length;
        Console.WriteLine($"{end}");
        // Does any number overlap with this seed?
        var overlaps = numbers.Where(n => n.Start <= end && n.Start + n.Length <= start);

        if (overlaps.Any())
        {
            Console.WriteLine($"Overlap found: {start} - {end}");
        }
        index++;
    }
}
internal class Maps
{
    public Map Seed2Soil { get; set; } = new Map();
    public Map Soil2Fertilizer { get; set; } = new Map();
    public Map Fertilizer2Water { get; set; } = new Map();
    public Map Water2Light { get; set; } = new Map();
    public Map Light2Temperature { get; set; } = new Map();
    public Map Temperature2Humidity { get; set; } = new Map();
    public Map Humidity2Location { get; set; } = new Map();
}

internal class Map
{
    private List<(uint Destionation, uint Source, uint Length)> Mapping { get; set; } = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Get(uint val)
    {
        // if val is within the range of sourceStart and sourceStart + length, then get the delta between
        // start and val, and add it to destionationStart
        // otherwise, return same value

        foreach (var (sourceStart, destinationStart, length) in Mapping)
        {
            if (val >= sourceStart && val <= sourceStart + length)
            {
                return destinationStart + (val - sourceStart);
            }
        }

        return val;

    }

    internal void AddMapping(uint sourceStart, uint destinationStart, uint length)
    {
        Mapping.Add((sourceStart, destinationStart, length));
    }
}