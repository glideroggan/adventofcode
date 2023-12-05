using System.Diagnostics;
using System.Text;

class Tree
{
    public readonly Folder Root;
    private Folder? _currentFolder;

    public Tree()
    {
        Root = new Folder("/", null);
        _currentFolder = Root;
    }
    public void NavigateToRoot()
    {
        _currentFolder = Root;
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

        NavigateAndPrint(sb, Root, 0);
        return sb.ToString();
    }

    private void NavigateAndPrint(StringBuilder sb, Folder folder, int depth)
    {
        sb.Append("\n");
        sb.Append("".PadRight(depth, ' '));
        sb.Append("- ");
        sb.Append($"{folder.Name} (dir, size={folder.Size})");
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
        var folders = Root.Check(predicate);
        return folders;
    }
}