using System.Text.RegularExpressions;

var rawInput = await ReadInput();

async Task<string[]> ReadInput()
{
    return await File.ReadAllLinesAsync("input.txt");
}

var result = Day05.Solve(rawInput);
Console.WriteLine($"Message: '{result}'");

static class Day05
{
    public static string Solve(string[] rawInput)
    {
        /*
         * parse the stacks
         */

        var (instructions, stacks) = ParseStacks(rawInput);

        FollowInstructions(instructions, stacks);

        return stacks.Aggregate("", (current, t) => current + t.Peek());
    }

    private static void FollowInstructions(string[] instructions, Stack<char>[] stacks)
    {
        var reg = new Regex(@"move (\d+) from (\d+) to (\d+)",
            RegexOptions.Compiled | RegexOptions.NonBacktracking | RegexOptions.IgnoreCase);
        foreach (var line in instructions)
        {
            var match = reg.Match(line);
            var numOfItemsToMove = int.Parse(match.Groups[1].Value);
            var fromStack = int.Parse(match.Groups[2].Value)-1;
            var toStack = int.Parse(match.Groups[3].Value)-1;

            var queue = new Stack<char>();
            for (var i = 0; i < numOfItemsToMove; i++)
            {
                queue.Push(stacks[fromStack].Pop());
            }
            for (var i = 0; i < numOfItemsToMove; i++)
            {
                stacks[toStack].Push(queue.Pop());
            }
            
        }
    }

    private static (string[] instructions, Stack<char>[] stacks) ParseStacks(string[] rawInput)
    {
        var (stackIndexStart, maxStacks, instructions) = CreateStacks(rawInput);

        var stacks = new Stack<char>[maxStacks];
        // populate stacks
        
        for (var index = stackIndexStart; index >= 0; index--)
        {
            var span = rawInput[index].AsSpan();
            if (span[1] == '1') break;
            var stackNum = -1;
            // read 3 chars, then space
            for (var i = 0; i < span.Length; i += 4)
            {
                stackNum++;
                if (span[i + 1] == ' ') continue;

                stacks[stackNum] ??= new Stack<char>();
                stacks[stackNum]!.Push(span[i + 1]);
            }
        }

        return (instructions, stacks);
    }

    private static (int stackIndexStart, int numStacks, string[] instructions) CreateStacks(string[] rawInput)
    {
        for (var i = 0; i < rawInput.Length; i++)
        {
            if (rawInput[i][1] != '1') continue;

            var stackIndexStart = i - 1;
            // rest of the instructions
            var instructions = rawInput[(i + 2)..];

            // last stack number
            var index = rawInput[i].TrimEnd().LastIndexOf(' ') + 1;
            var noStr = rawInput[i][index..];
            var stackNo = int.Parse(noStr);
            return (stackIndexStart, stackNo, instructions);
        }

        throw new Exception("Input not valid");
    }
}