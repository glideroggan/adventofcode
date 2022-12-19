using System.Diagnostics;

var rawInput = await ReadInput();

async Task<string[]> ReadInput()
{
    return await File.ReadAllLinesAsync("input.txt");
}

var total = Day03.Solve(rawInput);

Console.WriteLine($"Sum of priorities: {total}");

class Day03
{
    public static int Solve(string[] rawInput)
    {
        var total = 0;
        foreach (var line in rawInput)
        {
            /*
             * go through the line (whole line)
             * first char, check the other half for it, every char iterated over gets indexed and moves the pointer
             * if not found at all, then all chars in compartment 2 is indexed and 
             */
            var span = line.AsSpan();
            var middle = span.Length / 2;
            var checkingCompartment = span[..middle];
            var duplicates = new List<char>();
            var currentIndex = middle;
            
            var hashs = new[] { new HashSet<char>(), new HashSet<char>() };
            int GetIndex(int i) => i < middle ? 1 : 0; 
            for (var i = 0; i < span.Length; i++)
            {
                if (duplicates.Contains(span[i])) continue;
                
                // check if span[i] is part of other compartment, first already indexed
                if (hashs[GetIndex(i)].Contains(span[i]))
                {
                    Debug.Assert(!duplicates.Contains(span[i]));
                    duplicates.Add(span[i]);
                }
                else if (currentIndex < span.Length)
                {
                    // check and index at the same time
                    for (var index = currentIndex; index < span.Length; index++)
                    {
                        hashs[GetIndex(i)].Add(span[index]);
                        currentIndex++;
                        if (!hashs[GetIndex(i)].Contains(span[i])) continue;
                        
                        Debug.Assert(!duplicates.Contains(span[i]));
                        duplicates.Add(span[i]);
                        break;
                    }
                }
            }
            // extracted all the duplicates
            Console.WriteLine($"Duplicates found in rucksack: {duplicates.Aggregate("", (s, c) => s + c)}");
            // count their total priority
            var sum = duplicates.Aggregate(0, (current, item) => current + GetPrio(item));

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


