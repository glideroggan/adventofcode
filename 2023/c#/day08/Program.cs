using System.Diagnostics;
using System.Text.RegularExpressions;

var fileData = File.ReadAllLines("input.txt");
var instructions = ParseInstructions(fileData);
var nodeLookup = new Dictionary<string, Node>();
ParseNodes(fileData);

var steps = Search();


Console.WriteLine($"Steps: {steps}");


int Search()
{
    // travel each starting node simultaneously, checking between each step if they are all on an ending node
    var startingPlaces = nodeLookup.Where(x => x.Value.Name.EndsWith('A')).Select(x => x.Value).ToList();
    Console.WriteLine($"Starting places: {startingPlaces.Count}");
    var entities = new List<Node>();
    foreach (var startingPlace in startingPlaces)
    {
        entities.Add(startingPlace);
    }
    var timer = new Stopwatch();
    timer.Start();
    var steps = 0;
    while (true) {
        steps++;
        var ending = true;
        for (var i = 0; i < entities.Count; i++)
        {
            if (instructions.Current == 'L')
            {
                entities[i] = nodeLookup[entities[i].Left];
                if (!entities[i].Name.EndsWith('Z')) ending = false;
                continue;
            }
            entities[i] = nodeLookup[entities[i].Right];
            if (!entities[i].Name.EndsWith('Z')) ending = false;
        }
        if (ending) break;
        instructions.Progress();

        if (timer.ElapsedMilliseconds > 1000)
        {
            Console.WriteLine($"Steps: {steps}");
            timer.Restart();
        }
    }

    return steps;
}


void ParseNodes(string[] fileData)
{
    var regex = new Regex(@"(\w+) = \((\w+), (\w+)\)");
    foreach (var line in fileData.Skip(2))
    {
        var parts = regex.Match(line);
        var name = parts.Groups[1].Value;
        var leftName = parts.Groups[2].Value;
        var rightName = parts.Groups[3].Value;

        var node = new Node
        {
            Name = name,
            Left = leftName,
            Right = rightName
        };

        nodeLookup.Add(name, node);
    }

    Console.WriteLine($"Node count: {nodeLookup.Count}");
}

static Instruction ParseInstructions(string[] fileData)
{
    return new Instruction { Data = fileData[0] };
}

struct Instruction
{
    public string Data { get; init; }
    private int index;
    public readonly char Current => Data[index];
    public void Progress()
    {
        index++;
        if (index >= Data.Length)
        {
            index = 0;
        }
    }
}

class Node
{
    public required string Name { get; set; }
    public required string Left { get; set; }
    public required string Right { get; set; }
}

