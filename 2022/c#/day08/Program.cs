var rawInput = await ReadInput();

async Task<string[]> ReadInput()
{
    return await File.ReadAllLinesAsync("input.txt");
}

var result = Day08.SolvePart2(rawInput);
Console.WriteLine($"Best scenic score: {result}");

static class Day08
{
    public static int Solve(string[] rawInput)
    {
        /*
         * How many trees are visible looking from all directions?
         *
         *  We should be able to calculate this in O(n) time, by keeping track from each side what the tallest number
         * is.
         * So by starting with first line, all trees are visible, and the tallest so far from left and top is the first
         * tree 3
         * left: visible (no trees)
         * top: visible (no trees)
         * right: start from right and any number higher than 3 means it is not visible (save highest tree)
         * next tree is 0, hidden from left (as 3 is higher than 0), visible from top (as there are no trees)
         * 
         */
        var visibleTrees = 0;
        var maxRight = rawInput[0].Length-1;
        var maxBottom = rawInput.Length-1;

        var tallestTop = new int[maxRight+1];
        for (var y = 0; y < rawInput.Length; y++)
        {
            var tallestBottom = new int[maxRight+1];
            var tallestLeft = 0;
            
            for (var x = 0; x < rawInput[y].Length; x++)
            {
                var tallestRight = 0;
                var current = int.Parse(rawInput[y][x].ToString());

                // update left record
                if (current > tallestLeft)
                {
                    tallestLeft = current;
                    if (current > tallestTop[x])
                    {
                        tallestTop[x] = current;
                    }
                    visibleTrees++;
                    continue;
                }
                // update top record
                if (current > tallestTop[x])
                {
                    tallestTop[x] = current;
                    if (current > tallestLeft)
                    {
                        tallestLeft = current;
                    }
                    visibleTrees++;
                    continue;
                }

                // check edges
                if (CheckEdges(x, y, maxRight, maxBottom, ref visibleTrees)) continue;
                
                // we should continue checking on the right side
                for (var r = x + 1; r < rawInput[y].Length; r++)
                {
                    if (rawInput[y][r] > tallestRight) tallestRight = int.Parse(rawInput[y][r].ToString());
                    if (tallestRight >= current)
                    {
                        break;
                    }
                }

                if (current > tallestRight)
                {
                    // must be visible from right
                    visibleTrees++;
                    continue;
                }
                
                
                // we should continue checking on the right side
                for (var b = y + 1; b < rawInput.Length; b++)
                {
                    if (rawInput[b][x] > tallestBottom[x]) tallestBottom[x] = int.Parse(rawInput[b][x].ToString());
                    if (tallestBottom[x] >= current)
                    {
                        break;
                    }
                }

                if (current > tallestBottom[x])
                {
                    // must be visible from bottom
                    visibleTrees++;
                    continue;
                }
            }
        }

        return visibleTrees;
    }

    private static bool CheckEdges(int x, int y, int maxRight, int maxBottom, ref int visibleTrees)
    {
        // check left
        if (x == 0)
        {
            visibleTrees++;
            return true;
        }

        // check top
        if (y == 0)
        {
            visibleTrees++;
            return true;
        }

        // check right
        if (x == maxRight)
        {
            visibleTrees++;
            return true;
        }

        // check bottom
        if (y == maxBottom)
        {
            visibleTrees++;
            return true;
        }

        return false;
    }

    public static (int, int, int) SolvePart2(string[] rawInput)
    {
        /*
         * Find best viewing distance by checking each tree how far you can reach in all directions
         * stopping at an each height tree or taller.
         *
         * 
         */

        var grid = ConvertToGrid(rawInput);
        var bestScore = (0, 0, 0);

        for (var y = 1; y < grid.GetLength(0)-1; y++)
        for (var x = 1; x < grid.GetLength(1)-1; x++)
        {
            var score = DistLeft(x, y, grid) * DistRight(x, y, grid) * DistUp(x, y, grid) * DistDown(x, y, grid);
            if (score > bestScore.Item3)
            {
                bestScore = (x, y, score);
            }
        }

        return bestScore;
    }

    private static int DistLeft(int x, int y, int[,] grid)
    {
        var current = grid[y, x];
        var distance = 0;
        for (var left = x-1; left >= 0; left--)
        {
            distance++;
            if (grid[y, left] >= current) break;
        }

        return distance;
    }
    private static int DistRight(int x, int y, int[,] grid)
    {
        var current = grid[y, x];
        var distance = 0;
        for (var right = x+1; right < grid.GetLength(1); right++)
        {
            distance++;
            if (grid[y, right] >= current) break;
        }

        return distance;
    }
    private static int DistUp(int x, int y, int[,] grid)
    {
        var current = grid[y, x];
        var distance = 0;
        for (var up = y-1; up >= 0; up--)
        {
            distance++;
            if (grid[up, x] >= current) break;
        }

        return distance;
    }
    private static int DistDown(int x, int y, int[,] grid)
    {
        var current = grid[y, x];
        var distance = 0;
        for (var down = y+1; down < grid.GetLength(0); down++)
        {
            distance++;
            if (grid[down, x] >= current) break;
        }

        return distance;
    }

    private static int[,] ConvertToGrid(string[] rawInput)
    {
        var grid = new int[rawInput.Length, rawInput[0].Length];
        for (var y = 0; y < rawInput.Length; y++)
        for (var x = 0; x < rawInput[y].Length; x++)
        {
            grid[y, x] = int.Parse(rawInput[y][x].ToString());
        }

        return grid;
    }
}

