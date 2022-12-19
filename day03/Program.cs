using System.Diagnostics;

var rawInput = await ReadInput();

async Task<string[]> ReadInput()
{
    return await File.ReadAllLinesAsync("input.txt");
}

var total = Day03.Solve(rawInput);

Console.WriteLine($"Sum of priorities: {total}");

static class Day03
{
    public static int Solve(string[] rawInput)
    {
        var total = 0;
        var lineCount = 0;
        var rucksack1 = new HashSet<char>();
        var rucksack2 = new HashSet<char>();
        var rucksack3 = new HashSet<char>();
        foreach (var line in rawInput)
        {
            var span = line.AsSpan();
            lineCount++;
            /*
             * go through each character in rucksack, each item gets indexed
             * 
             */

            for (var i = 0; i < span.Length; i++)
            {
                if (lineCount % 3 == 1)
                {
                    // first rucksack
                    rucksack1.Add(span[i]);
                }
                else if (lineCount % 3 == 2)
                {
                    // second
                    rucksack2.Add(span[i]);
                }
                else if (lineCount % 3 == 0)
                {
                    // third
                    rucksack3.Add(span[i]);
                }
            }
            
            if (lineCount % 3 != 0) continue;
            
            rucksack1.IntersectWith(rucksack2);
            rucksack1.IntersectWith(rucksack3);
            
            // extracted all the duplicates
            Console.WriteLine($"Duplicates found in all rucksacks: {rucksack1.Aggregate("", (s, c) => s + c)}");
            
            // count their total priority
            var sum = rucksack1.Aggregate(0, (current, item) => current + GetPrio(item));
            
            // clear for next round
            rucksack1.Clear();
            rucksack2.Clear();
            rucksack3.Clear();
            lineCount = 0;

            total += sum;
        }

        return total;
    }

    private static int GetPrio(char item)
    {
        return (int)item switch
        {
            (>= 97) and (<= 122) => item - 96,
            (>= 65) and (<= 90) => item - 38,
        };
    }
}