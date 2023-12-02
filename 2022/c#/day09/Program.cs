var rawInput = await ReadInput();

async Task<string[]> ReadInput()
{
    return await File.ReadAllLinesAsync("training.txt");
}

var result = Day09.Part1(rawInput);

static class Day09
{
    public static int Part1(string[] rawInput)
    {
        /*
         * Count number of positions in the grid that the tail visited at least once
         * The grid needs to be dynamic, as we don't know the starting position
         * The tail always want to be "close", in diagonally cases the tail want to be on
         * the same row or column
         * this
         * T..   ...
         * .H.   .TH
         * instead of this
         * T..   .T.
         * .H.   ..H 
         */

        var head = new Pos(0, 0);
        var tail = head;
        var visitedPositions = new HashSet<Pos>();
        
        foreach (var line in rawInput)
        {
            var amount = int.Parse(line[(line.IndexOf(' ') + 1)..]);
            var dir = line[..line.IndexOf(' ')] switch
            {
                "R" => new Pos(1, 0),
                "L" => new Pos(-1, 0),
                "U" => new Pos(0, -1),
                "D" => new Pos(0, 1)
            };
            while (amount > 0)
            {
                head = MoveHead(head, dir);
                tail = MoveTail(tail, head, dir);
                amount--;
                visitedPositions.Add(tail);
            }
        }
        return visitedPositions.Count;
    }

    private static Pos MoveTail(Pos tail, Pos head, Pos dir)
    {
        // if distance between head and tail are 1, then no need to move
        if (tail.Dist(head) <= 1) return tail;
        if (tail.X == head.X || tail.Y == head.Y)
        {
            // at the same row or column, so we can just move normal
            return new Pos(X: tail.X + dir.X, Y: tail.Y + dir.Y);
        }

        // we need to move into the same column or row
        if (tail.X + 1 == head.X) return tail with { X = tail.X + 1 };
        if (tail.X - 1 == head.X) return tail with { X = tail.X - 1 };
        if (tail.Y - 1 == head.Y) return tail with { Y = tail.Y - 1 };
        if (tail.Y + 1 == head.Y) return tail with { Y = tail.Y + 1 };

        throw new Exception("Wrong");
    }

    private static Pos MoveHead(Pos head, Pos dir)
    {
        return new Pos(X: head.X + dir.X, Y: head.Y + dir.Y);
    }
}

public enum DirectionEnum
{
    Right,
    Left,
    Up,
    Down
}
public record struct Pos(int X, int Y)
{
    public int Dist(Pos other)
    {
        var dx = other.X - X;
        var dy = other.Y - Y;
        return (int)Math.Sqrt(dx * dx + dy * dy);
    }
}