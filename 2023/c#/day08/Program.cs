using System.Diagnostics;
using System.Text.RegularExpressions;

var fileData = File.ReadAllLines("input.txt");
var instructions = ParseInstructions(fileData);
var nodeLookup = new Dictionary<string, Node>();
ParseNodes(fileData);

var steps = Search();


Console.WriteLine($"Steps: {steps}");


long Search()
{
    // travel each starting node simultaneously, checking between each step if they are all on an ending node
    var startingPlaces = nodeLookup.Where(x => x.Value.Name[^1] == 'A').Select(x => x.Value).ToList();
    Console.WriteLine($"Starting places: {startingPlaces.Count}");
    var endingPlaces = nodeLookup.Where(x => x.Value.Name[^1] == 'Z').Select(x => x.Value.Name).ToHashSet();
    Console.WriteLine($"Ending places: {endingPlaces.Count}, {string.Join(", ", endingPlaces.Select(x => x.ToString()))}");

    /* What if I map each instruction step to a landing node? Shouldn't I be able to tell then which instruction will contain the ending node?
    */



    var entities = new Node[startingPlaces.Count];
    for (var i = 0; i < startingPlaces.Count; i++)
    {
        entities[i] = startingPlaces[i];
    }
    var timer = new Stopwatch();
    timer.Start();
    var steps = 0L;
    var stepsPerSecond = 0;
    while (true)
    {
        steps++;
        stepsPerSecond++;
        if (instructions.Current == 'L')
        {
            for (var i = 0; i < startingPlaces.Count; i++)
            {
                entities[i] = entities[i].Left!;
            }
        }
        else 
        {
            for (var i = 0; i < startingPlaces.Count; i++)
            {
                entities[i] = entities[i].Right!;
            }
        }
        // check if name ends with Z
        var ending = true;
        for (var i = 0; i < startingPlaces.Count; i++)
        {
            if (entities[i].Name[^1] != 'Z')
            {
                ending = false;
                break;
            }
        }
        if (ending)
        {
            break;
        }

        instructions.Index++;
        if (timer.ElapsedMilliseconds > 1000)
        {
            Console.WriteLine($"Steps: {steps}, Steps per second: {stepsPerSecond} s/s, Turn: {instructions.Current}");
            timer.Restart();
            stepsPerSecond = 0;
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
    public int Index
    {
        readonly get => index;
        set
        {
            index = value;
            if (index >= Data.Length)
            {
                index = 0;
            }
        }
    }
    private int index;
    public readonly char Current => Data[index];
}

class Node
{
    public required string Name { get; init; }
    public Node? Left { get; set; }
    public Node? Right { get; set; }
}