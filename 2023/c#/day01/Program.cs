// read input
using System.Diagnostics;

var lines = File.ReadAllLines("input.txt");

var digits = new List<int>();

// find the single digit on each side of the string
// we don't know which number it is
// we don't know if there are more than one

// start with left side and go through 0..9 and what ever position is the lowest, is the number

var leftSide = int.MaxValue;
var rightSide = int.MinValue;

var characterDigits = new string[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
Span<char> span = stackalloc char[2];
foreach (var line in lines)
{
    var leftSideIsString = false;
    var rightSideIsString = false;
    var spanLine = line.AsSpan();

    // left side
    var position = spanLine.IndexOfAny(numbers);
    leftSide = position != -1 ? position : int.MaxValue;
    // search for character digits also
    for (int i = 0; i < characterDigits.Length; i++)
    {
        position = spanLine.IndexOf(characterDigits[i]);
        if (position != -1 && position < leftSide)
        {
            leftSide = position;
            // parse out the number
            leftSideIsString = true;
            span[0] = TransformToNumber(i);
        }
    }
    // right side
    position = line.AsSpan().LastIndexOfAny(numbers);
    rightSide = position != -1 ? position : int.MinValue;

    // search for character digits also
    for (int i = 0; i < characterDigits.Length; i++)
    {
        position = line.AsSpan().LastIndexOf(characterDigits[i]);
        if (position != -1 && position > rightSide)
        {
            rightSide = position;
            // parse out the number
            rightSideIsString = true;
            span[1] = TransformToNumber(i);
        }
    }

    // take the two numbers and fuse to one
    if (!leftSideIsString)
    {
        span[0] = line.AsSpan().Slice(leftSide, 1)[0];
    }
    if (!rightSideIsString)
    {
        span[1] = line.AsSpan().Slice(rightSide, 1)[0];
    }

    Console.WriteLine($"span: {span.ToString()}");
    var number = int.Parse(span);
    digits.Add(number);
}

static char TransformToNumber(int i) =>
    i switch
    {
        0 => '0',
        1 => '1',
        2 => '2',
        3 => '3',
        4 => '4',
        5 => '5',
        6 => '6',
        7 => '7',
        8 => '8',
        9 => '9',
        _ => throw new Exception("Not a number")
    };

// print out the numbers
var sum = digits.Sum();
Console.WriteLine($"Sum of all numbers: {sum}");

partial class Program
{
    private static readonly System.Buffers.SearchValues<char> numbers = System.Buffers.SearchValues.Create("0123456789");
}
