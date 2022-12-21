using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

var rawInput = await ReadInput();

async Task<string[]> ReadInput()
{
    return await File.ReadAllLinesAsync("input.txt");
}

var result = Day07.Solve(rawInput);
Console.WriteLine(result);

// find folders with a size < 100.000
var folders = result.GetFolders(f => f.Size < 100_000);
Console.WriteLine($"Total of size of those folder: {folders.Sum(f => f.Size)}");
// Console.WriteLine($"Folders: {folders.Aggregate("", (s, folder) => s + $"\n{folder.Name}")}");


static class Day07
{
    internal static Tree Solve(string[] rawInput)
    {
        /*
         * re-create disk tree by parsing the terminal
         * $ - will be a command
         *  ls - will show what content we have in current folder position
         *  cd <folder> - will navigate in tree, meaning that we should create this folder in the tree
         *  cd .. - go one up in the tree
         *
         * regex:
         * commands: "\$ (\w+).(.*)" command, args
         */
        var commandRegex = new Regex(@"\$ (\w+).(.*)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.NonBacktracking);
        var contentRegex = new Regex(@"^(dir|\d+) (.*)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.NonBacktracking);
        var tree = new Tree();
        foreach (var line in rawInput)
        {
            var span = line.AsSpan();
            
            var isCommand = commandRegex.IsMatch(span);
            var isContent = contentRegex.IsMatch(span); 
            if (isCommand)
            {
                HandleCommand(commandRegex, span, tree);
            }
            else if (isContent)
            {
                HandleContent(contentRegex, span, tree);
            }
        }

        return tree;
    }

    private static void HandleContent(Regex contentRegex, ReadOnlySpan<char> span, Tree tree)
    {
        var match = contentRegex.Match(span.ToString());
        if (int.TryParse(match.Groups[1].Value, out var size))
        {
            tree.CreateFile(size, match.Groups[2].Value);
        }
        else
        {
            tree.CreateFolder(match.Groups[2].Value);
        }
    }

    private static void HandleCommand(Regex commandRegex, ReadOnlySpan<char> span, Tree tree)
    {
        var match = commandRegex.Match(span.ToString());
        var cmd = match.Groups[1].Value;
        switch (cmd.ToLowerInvariant())
        {
            case "cd":
                var arg = match.Groups[2].Value;
                if (arg == "..")
                {
                    tree.Up();
                }
                else if (arg == "/")
                {
                    tree.Root();
                }
                else
                {
                    tree.CreateAndNavigate(arg);
                }

                break;
            case "ls":
                // following parsing will be content
                break;
        }
    }
}

class Tree
{
    private readonly Folder _root;
    private Folder? _currentFolder;

    public Tree()
    {
        _root = new Folder("/", null);
        _currentFolder = _root;
    }
    public void Root()
    {
        _currentFolder = _root;
    }

    public void Up()
    {
        Debug.Assert(_currentFolder?.Parent != null);
        _currentFolder = _currentFolder.Parent;
    }

    public void CreateAndNavigate(string folderName)
    {
        _currentFolder = _currentFolder.Create(folderName); 
    }

    public void CreateFolder(string folderName)
    {
        _currentFolder.Create(folderName);
    }

    public void CreateFile(int size, string fileName)
    {
        _currentFolder.AddFile(size, fileName);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        NavigateAndPrint(sb, _root, 0);
        return sb.ToString();
    }

    private void NavigateAndPrint(StringBuilder sb, Folder folder, int depth)
    {
        sb.Append("\n");
        sb.Append("".PadRight(depth, ' '));
        sb.Append("- ");
        sb.Append($"{folder.Name} (dir)");
        foreach (var subFolder in folder.Children)
        {
            NavigateAndPrint(sb, subFolder, depth+2);
        }

        foreach (var file in folder.Files)
        {
            PrintFile(sb, file, depth+2);
        }
    }

    private void PrintFile(StringBuilder sb, (string FileName, int Size) file, int depth)
    {
        sb.Append("\n");
        sb.Append("".PadRight(depth, ' '));
        sb.Append("- ");
        sb.Append($"{file.FileName} (file, size={file.Size})");
    }

    public Folder[] GetFolders(Func<Folder, bool> predicate)
    {
        var folders = _root.Check(predicate);
        return folders;
    }
}

class Folder
{
    public string Name;
    public readonly Folder? Parent;
    public readonly List<Folder> Children;
    public List<(string FileName, int Size)> Files;

    public Folder(string name, Folder? parent)
    {
        Name = name;
        Parent = parent;
        Children = new List<Folder>();
        Files = new List<(string FileName, int Size)>();
    }

    public int Size
    {
        get
        {
            return Children.Sum(subFolder => subFolder.Size) + Files.Sum(file => file.Size);
        }
    }

    public Folder Create(string folderName)
    {
        if (Children.Exists(f => f.Name == folderName)) return Children.First(f => f.Name == folderName);
        
        var f = new Folder(folderName, this);
        Children.Add(f);
        return f;
    }

    public void AddFile(int size, string fileName)
    {
        Files.Add((fileName, size));
    }

    public Folder[] Check(Func<Folder, bool> predicate)
    {
        var res = new List<Folder>();
        if (predicate(this)) res.Add(this);
        foreach (var subFolder in Children)
        {
            res.AddRange(subFolder.Check(predicate));
        }

        return res.ToArray();
    }
}