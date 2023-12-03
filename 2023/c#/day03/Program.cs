using System.Globalization;

var fileData = ReadInput("input.txt");

// process data as a grid
var validPartNumbers = GetParts(fileData);

// output result
Console.WriteLine($"Sum of valid part numbers: {validPartNumbers.Sum()}");

static int[] GetParts(string[] fileData)
{
    // start on position up left
    // parse until number, then until non-number
    // but each number, check surroundings for a symbol

    var data = string.Concat(fileData);
    // Console.WriteLine($"Data: {data}");

    var stride = fileData[0].Length;
    var index = -1;
    var partNumbers = new List<int>();
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
    var directionStr = string.Empty;
    // var indexPos = (x: 0, y: 0);


    // if I overshoot a stride, then I should always do the number check
    while (index < data.Length-1)
    {
        var current = data[++index];
        if (index % stride == 0 && index >= stride) {
            // Console.WriteLine($"Overshoot: index[{index}], current[{current}], str[{numberStr}], ");
            if (inNumber && isPart) {
                partNumbers.Add(int.Parse(numberStr, CultureInfo.InvariantCulture));
                numberStr = string.Empty;
                inNumber = false;
                isPart = false;
            }
            else if (inNumber && !isPart) {
                numberStr = string.Empty;
                inNumber = false;
                isPart = false;
            }
        }
        

        switch (current) {
            case { } when inNumber && !isPart && !digits.Contains(current):
                numberStr = string.Empty;
                inNumber = false;
                isPart = false;
                continue;
            case { } when inNumber && isPart && !digits.Contains(current):
                partNumbers.Add(int.Parse(numberStr, CultureInfo.InvariantCulture));
                numberStr = string.Empty;
                inNumber = false;
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
        if (inNumber && !isPart) {
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
                    // Console.WriteLine($"Found part number: {numberStr}, " +
                    //     $"direction: {direction}");
                    directionStr = direction.ToString();
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