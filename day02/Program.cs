var rawInput = await ReadInput();

async Task<string[]> ReadInput()
{
    return await File.ReadAllLinesAsync("input.txt");
}


var totalScore = 0;
foreach (var line in rawInput)
{
    var opponent = (line[0]) switch {
        'A' => "Rock",
        'B' => "Paper",
        'C' => "Scissors"
    };
    var you = (line[^1]) switch {
        'X' => "Rock",
        'Y' => "Paper",
        'Z' => "Scissors"
    };
    var yourScore = (opponent, you) switch {
        ("Rock","Rock") => 3 + 1,
        ("Rock", "Paper") => 6 + 2,
        ("Rock", "Scissors") => 0 + 3,
        ("Paper", "Rock") => 0 + 1,
        ("Paper", "Paper") => 3 + 2,
        ("Paper", "Scissors") => 6 + 3,
        ("Scissors", "Rock") => 6 + 1,
        ("Scissors", "Paper") => 0 + 2,
        ("Scissors", "Scissors") => 3 + 3
    };

    totalScore += yourScore;
}

Console.WriteLine($"Total score: {totalScore}");

