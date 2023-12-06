
var dataLines = File.ReadAllLines("input.txt");

var seeds = ParseSeeds(dataLines[0]);

static uint[] ParseSeeds(string line)
{
    var parts = line.Split(':');
    var seeds = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    return seeds.Select(s => s.Trim()).Select(s => uint.Parse(s)).ToArray();
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
var locations = new List<uint>();
foreach (var seed in seeds)
{
    var soil = maps.Seed2Soil.Get(seed);
    var fertilizer = maps.Soil2Fertilizer.Get(soil);
    var water = maps.Fertilizer2Water.Get(fertilizer);
    var light = maps.Water2Light.Get(water);
    var temperature = maps.Light2Temperature.Get(light);
    var humidity = maps.Temperature2Humidity.Get(temperature);
    var location = maps.Humidity2Location.Get(humidity);
    locations.Add(location);
}

// lowest location
Console.WriteLine($"Lowest location: {locations.Min()}");

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
    public List<(uint Destionation, uint Source, uint Length)> Mapping { get; set; } = 
        [];
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