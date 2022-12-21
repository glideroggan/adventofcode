var rawInput = await ReadInput();

async Task<string[]> ReadInput()
{
    return await File.ReadAllLinesAsync("input.txt");
}

var result = Day06.Solve(rawInput);
Console.WriteLine($"Message: '{result.Aggregate("", (s, i) => s + $"{i}\n")}");

static class Day06
{
    internal static int[] Solve(string[] rawInput)
    {
        /*
         * There need to be 4 unique characters in a row
         */

        var positions = new List<int>();
        foreach (var line in rawInput)
        {
            var span = line.AsSpan();
            
            var lastChars = new char[14];
            var cursor = 0;
            for (var i = 0; i < 14; i++)
            {
                lastChars.Enqueue(span[cursor++]);    
            }
            
            do
            {
                if (lastChars.AllUnique()) break;
                lastChars.Enqueue(span[cursor++]);

            } while (true);
            
            positions.Add(cursor);
        }

        return positions.ToArray();
    }
}

static class CharArrayExtensions
{
    public static void Enqueue(this char[] arr, char c)
    {
        for (var i = arr.Length - 1; i > 0; i--)
        {
            arr[i] = arr[i - 1];    
        }

        arr[0] = c;
    }

    public static bool AllUnique(this char[] arr)
    {
        for (var a = 0; a < arr.Length - 1; a++)
        for (var b = a + 1; b < arr.Length; b++)
        {
            if (arr[a] == arr[b]) return false;
        }

        return true;
    }
    
}