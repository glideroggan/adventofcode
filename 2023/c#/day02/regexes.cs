using System.Text.RegularExpressions;

namespace day02;

public static partial class Regexes
{
    
    [GeneratedRegex(@"(?<val>\d+) (?<color>\w+)")]
    public static partial Regex ColorAndValueRegex();
}