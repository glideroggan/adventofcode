using System.Globalization;
using System.Text.Json;

var fileData = ReadInput("input.txt");

// process data as a grid
var validPartNumbers = GetParts(fileData);

// find out which numbers that "touch" the same gear symbol *
var partGears = validPartNumbers
    .Where(p => p.Symbol == '*')
    // .Select(p => (p.Number, p.SymbolPos))
    .GroupBy(p => p.SymbolPos)
    .Where(g => g.Count() > 1)
    // .SelectMany(g => g.Select(p => p))
    // .Distinct()
    .ToArray();

// Console.WriteLine($"Part gears: {JsonSerializer.Serialize(validPartNumbers)}");
// Console.WriteLine($"Part gears: {JsonSerializer.Serialize(partGears)}");
// multiply gear numbers within the same group, and sum all groups
var result = partGears
    .Select(g => g.Select(p => p.Number).Aggregate((a, b) => a * b))
    .Sum();


// output result
Console.WriteLine($"Sum of valid part numbers: {result}");

static Part[] GetParts(string[] fileData)
{
    // start on position up left
    // parse until number, then until non-number
    // but each number, check surroundings for a symbol

    var data = string.Concat(fileData);
    // Console.WriteLine($"Data: {data}");

    var stride = fileData[0].Length;
    var index = -1;
    var partNumbers = new List<Part>();
    var digits = new HashSet<char>("0123456789");
    // TODO: can we go through to next stride from right?, or left?
    var directions = new[] {
        -stride - 1, -stride, -stride + 1,
        -1, 1,
        stride - 1, stride, stride + 1
    };
    var inNumber = false;
    var numberStr = string.Empty;
    var isPart = false;
    var symbolPos = -1;


    // if I overshoot a stride, then I should always do the number check
    while (index < data.Length - 1)
    {
        var current = data[++index];
        if (index % stride == 0 && index >= stride)
        {
            if (inNumber && isPart)
            {
                var part = new Part
                {
                    Number = int.Parse(numberStr, CultureInfo.InvariantCulture),
                    SymbolPos = symbolPos,
                    Symbol = data[symbolPos]
                };
                partNumbers.Add(part);
                numberStr = string.Empty;
                inNumber = false;
                isPart = false;
                symbolPos = -1;
            }
            else if (inNumber && !isPart)
            {
                numberStr = string.Empty;
                inNumber = false;
                isPart = false;
            }
        }


        switch (current)
        {
            case { } when inNumber && !isPart && !digits.Contains(current):
                numberStr = string.Empty;
                inNumber = false;
                isPart = false;
                continue;
            case { } when inNumber && isPart && !digits.Contains(current):
                var part = new Part
                {
                    Number = int.Parse(numberStr, CultureInfo.InvariantCulture),
                    SymbolPos = symbolPos,
                    Symbol = data[symbolPos]
                };
                partNumbers.Add(part);
                numberStr = string.Empty;
                inNumber = false;
                symbolPos = -1;
                isPart = false;
                continue;
            case { } when !inNumber && !digits.Contains(current):
                continue;
            case { } when !inNumber && digits.Contains(current):
                inNumber = true;
                numberStr += current;
                break;
            case { } when inNumber && digits.Contains(current) && !isPart:
                numberStr += current;
                break;
            case { } when inNumber && digits.Contains(current) && isPart:
                numberStr += current;
                continue;
            default: throw new Exception($"Unknown state: {current}");

        };

        // check surroundings
        if (inNumber && !isPart)
        {
            foreach (var direction in directions)
            {
                var checkIndex = index + direction;
                if (checkIndex < 0 || checkIndex >= data.Length)
                {
                    continue;
                }

                var check = data[checkIndex];
                if (!digits.Contains(check) && check != '.')
                {
                    isPart = true;
                    symbolPos = index + direction;
                    break;
                }
            }
        }
    }

    return partNumbers.ToArray();
}

static string[] ReadInput(string fileName)
{
    return File.ReadAllLines(fileName);
}

struct Part
{
    public int Number { get; set; }
    public int SymbolPos { get; set; }
    public char Symbol { get; set; }
}