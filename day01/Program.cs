// See https://aka.ms/new-console-template for more information

var rawInput = await ReadInput();

async Task<string[]> ReadInput()
{
    return await File.ReadAllLinesAsync("input.txt");
}

var calorieList = ParseInput(rawInput);

List<int> ParseInput(string[] rawInput)
{
    var elfList = new List<int>();
    var elfCalorieAmount = 0;
    foreach (var line in rawInput)
    {

        if (string.IsNullOrWhiteSpace(line)) {
            // new elf
            elfList.Add(elfCalorieAmount);
            elfCalorieAmount = 0;
            continue;
        }

        var val = int.Parse(line);
        elfCalorieAmount += val;
    }
    elfList.Add(elfCalorieAmount);

    elfList.Sort();
    return elfList;
}

Console.WriteLine($"Highest calorie amount carried: {calorieList[^1]}");