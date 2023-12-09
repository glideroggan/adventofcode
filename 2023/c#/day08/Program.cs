using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

var fileData = File.ReadAllLines("input.txt");
var instructions = ParseInstructions(fileData);
var nodeLookup = new Dictionary<string, Node>();

var steps = Search(fileData, instructions);


Console.WriteLine($"Steps: {steps}");


int Search(string[] fileData, Instruction instructions)
{
    // parse out the nodes
    var root = ParseNodes(fileData);

    Console.WriteLine($"Root: {JsonSerializer.Serialize(root)}");
    // travel the nodes by following the instructions
    var steps = 0;
    var current = root;
    while (current.Name != "ZZZ")
    {
        if (instructions.Current == 'L')
        {
            instructions.Progress();
            steps++;
            current = nodeLookup[current.Left];
            continue;
        }
        instructions.Progress();
        steps++;
        current = nodeLookup[current.Right];
    }
    return steps;
}


Node ParseNodes(string[] fileData)
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
    return nodeLookup["AAA"];
}

void Travel(Node node, ref int steps)
{
    if (node.Name == "ZZZ")
    {
        Console.WriteLine("Destination reached");
        return;
    }
    if (instructions.Current == 'L')
    {
        instructions.Progress();
        steps++;
        Travel(nodeLookup[node.Left], ref steps);
        return;
    }
    instructions.Progress();
    steps++;
    Travel(nodeLookup[node.Right], ref steps);
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

