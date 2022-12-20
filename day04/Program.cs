var rawInput = await ReadInput();

async Task<string[]> ReadInput()
{
    return await File.ReadAllLinesAsync("input.txt");
}

var result = Day04.Solve(rawInput);
Console.WriteLine($"There {result} pairs that are covered.");


static class Day04
{
    static bool IsRangeCovered(int range1Min, int range1Max, int range2Min, int range2Max)
    {
        return (range1Min >= range2Min && range1Max <= range2Max) || 
               (range2Min >= range1Min && range2Max <= range1Max);
    }

    public static int Solve(string[] rawInput)
    {
        /*
         * Check pairs
         * If one of the parts of the pair is covering fully the other, then count it
         */
        var count = 0;
        
        foreach (var line in rawInput)
        {
            var span = line.AsSpan();
            var part1 = span[..span.IndexOf(',')];
            var part2 = span[(span.IndexOf(',')+1)..];
            var part1NumMin = int.Parse(part1[..part1.IndexOf('-')]);
            var part1NumMax = int.Parse(part1[(part1.IndexOf('-')+1)..]);
            var part2NumMin = int.Parse(part2[..part2.IndexOf('-')]);
            var part2NumMax = int.Parse(part2[(part2.IndexOf('-')+1)..]);

            if (IsRangeCovered(part1NumMin, part1NumMax, part2NumMin, part2NumMax))
            {
                count++;
            }
        }

        return count;
    }
}