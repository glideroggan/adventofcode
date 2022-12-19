var rawInput = await ReadInput();

async Task<string[]> ReadInput()
{
    return await File.ReadAllLinesAsync("training.txt");
}

class Helper
{
    void Parse(string[] rawInput)
    {
        foreach (var line in rawInput)
        {
            var span = line.AsSpan();
            var comp1 = span[..(line.Length/2)];
            var comp2 = span[(line.Length/2)..];
            // we need to filter out those items that are in BOTH compartments
            
            
        }        
    }
}


