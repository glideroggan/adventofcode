using System.Diagnostics;
using System.Text.RegularExpressions;

var fileData = File.ReadAllLines("input.txt");
var instructions = ParseInstructions(fileData);
var nodeLookup = new Dictionary<string, Node>();
ParseNodes(fileData);

var steps = Search();


Console.WriteLine($"Steps: {steps}");


UInt64 Search()
{
    // travel each starting node simultaneously, checking between each step if they are all on an ending node
    var startingPlaces = nodeLookup.Where(x => x.Value.Name[^1] == 'A').Select(x => x.Value).ToList();
    Console.WriteLine($"Starting places: {string.Join(", ", startingPlaces.Select(x => x.Name))}");
    var endingPlaces = nodeLookup.Where(x => x.Value.Name[^1] == 'Z').Select(x => x.Value.Name).ToHashSet();
    Console.WriteLine($"Ending places: {string.Join(", ", endingPlaces.Select(x => x.ToString()))}");
    Console.WriteLine($"Instruction length: {instructions.Data.Length}");

    var jumps = new List<UInt64>();
    var entities = new Node[startingPlaces.Count];
    for (var i = 0; i < startingPlaces.Count; i++)
    {
        entities[i] = startingPlaces[i];
    }
    uint steps = 0;
    // First, find each starting point end point Z
    // then use the first two to calculate their LCM
    // use that result to calculate the third entity and so on
    for (var i = 0; i < startingPlaces.Count; i++)
    {
        var entity = entities[i];
        steps = 0;
        while (entity.Name[^1] != 'Z')
        {
            if (instructions.Data[(int)steps % instructions.Data.Length] == 'L')
            {
                entity = entity.Left!;
            }
            else
            {
                entity = entity.Right!;
            }
            steps++;
        }
        jumps.Add(steps);
        Console.WriteLine($"Entity {i} found Z in {steps} steps");
    }

    var lcm = jumps[0];
    for (var i = 1; i < jumps.Count; i++)
    {
        lcm = LCM(lcm, jumps[i]);
    }

    return lcm;

    static UInt64 LCM(UInt64 a, UInt64 b)
    {
        Console.WriteLine($"Calculating LCM of {a} and {b}");
        var gcd = GCD(a, b);
        UInt64 lcm = a / gcd * b;
        Console.WriteLine($"LCM: {lcm}");
        return lcm;
    }
    static UInt64 GCD(UInt64 a, UInt64 b)
    {
        Console.WriteLine($"Calculating GCD of {a} and {b}");
        while (b != 0)
        {
            var temp = b;
            b = a % b;
            a = temp;
        }
        Console.WriteLine($"GCD: {a}");
        return a;
    }
}

void ParseNodes(string[] fileData)
{
    var regex = new Regex(@"(\w+) = \((\w+), (\w+)\)");
    foreach (var line in fileData.Skip(2))
    {
        var parts = regex.Match(line);
        var name = parts.Groups[1].Value;


        var node = new Node
        {
            Name = name,
            Left = new Node { Name = parts.Groups[2].Value },
            Right = new Node { Name = parts.Groups[3].Value }
        };

        nodeLookup.Add(name, node);
    }

    // go through each node in the list and make sure it has a left and right node
    foreach (var node in nodeLookup.Values)
    {
        node.Left = nodeLookup[node.Left!.Name];
        node.Right = nodeLookup[node.Right!.Name];
    }

    Console.WriteLine($"Node count: {nodeLookup.Count}");
}

static Instruction ParseInstructions(string[] fileData)
{
    return new Instruction { Data = fileData[0] };
}

internal struct Instruction
{
    public string Data { get; init; }
}

class Node
{
    public required string Name { get; init; }
    public Node? Left { get; set; }
    public Node? Right { get; set; }
}