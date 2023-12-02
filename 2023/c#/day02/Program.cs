using System.Text.RegularExpressions;
using day02;

var gameInputs = ReadInput("input.txt");

var sum = 0;
foreach (var game in gameInputs)
{
    Console.WriteLine($"Game: id:{game.Id} red:{game.RedMax} green:{game.GreenMax} blue:{game.BlueMax}");
    sum += game.RedMax * game.GreenMax * game.BlueMax;
}

var answer = sum;

Console.WriteLine($"Answer: {answer}");
return;

static IEnumerable<GameInput> ReadInput(string filename)
{
    var lines = File.ReadAllLines(filename);
    foreach (var line in lines)
    {
        var ret = new GameInput();
        var parts = line.Split(':');
        var id = line.Substring(line.IndexOf(' ') + 1, line.IndexOf(':') - line.IndexOf(' ') - 1);
        ret.Id = int.Parse(id);

        var sets = parts[1].Split(';');
        foreach (var set in sets)
        {

            // split out the color and value
            var matches = Regexes.ColorAndValueRegex().Matches(set);
            foreach (Match match in matches)
            {
                var value = match.Groups["val"].Value;
                var color = match.Groups["color"].Value;

                switch (color)
                {
                    case "red":
                        ret.RedMax = int.Parse(value) > ret.RedMax ? int.Parse(value) : ret.RedMax;
                        break;
                    case "green":
                        ret.GreenMax = int.Parse(value) > ret.GreenMax ? int.Parse(value) : ret.GreenMax;
                        break;
                    case "blue":
                        ret.BlueMax = int.Parse(value) > ret.BlueMax ? int.Parse(value) : ret.BlueMax;
                        break;
                }
            }
        }

        yield return ret;
    }
}

internal struct GameInput
{
    public int Id { get; set; }
    public int RedMax { get; set; }
    public int GreenMax { get; set; }
    public int BlueMax { get; set; }
}



