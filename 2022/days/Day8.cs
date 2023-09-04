namespace Advent2022;

public static class Day8
{
    public static void Run(string[] data)
    {
        var width = data[0].Length;
        var height = data.Length;
        var map = string.Join("", data);

        var visCount = 0;
        var maxScore = 0;

        foreach (var y in Enumerable.Range(0, height))
        {
            foreach (var x in Enumerable.Range(0, width))
            {
                var index = x + y * width;

                var tree = int.Parse(map[index].ToString());

                //check if any trees from here to edges are shorter
                var lSeen = true;
                var rSeen = true;
                var tSeen = true;
                var bSeen = true;

                var lTrees = 0;
                var rTrees = 0;
                var tTrees = 0;
                var bTrees = 0;

                //left edge
                foreach (var x2 in Enumerable.Range(0, x).Reverse())
                {
                    var otherIndex = x2 + y * width;
                    var otherTree = int.Parse(map[otherIndex].ToString());
                    if (otherTree >= tree)
                    {
                        if (x != 0 || x != width)
                            lTrees = x - x2;

                        lSeen = false;
                        break;
                    }
                }

                if (lSeen)
                    lTrees = x;

                //right edge
                foreach (var x2 in Enumerable.Range(x + 1, width - x - 1))
                {
                    var otherIndex = x2 + y * width;
                    var otherTree = int.Parse(map[otherIndex].ToString());
                    if (otherTree >= tree)
                    {
                        if (x != 0 || x != width)
                            rTrees = x2 - x;

                        rSeen = false;
                        break;
                    }
                }

                if (rSeen)
                    rTrees = width - x - 1;

                //top edge
                foreach (var y2 in Enumerable.Range(0, y).Reverse())
                {
                    var otherIndex = x + y2 * width;
                    var otherTree = int.Parse(map[otherIndex].ToString());
                    if (otherTree >= tree)
                    {
                        if (y != 0 || y != height)
                            tTrees = y - y2;

                        tSeen = false;
                        break;
                    }
                }

                if (tSeen)
                    tTrees = y;

                //bottom edge
                foreach (var y2 in Enumerable.Range(y + 1, height - y - 1))
                {
                    var otherIndex = x + y2 * width;
                    var otherTree = int.Parse(map[otherIndex].ToString());
                    if (otherTree >= tree)
                    {
                        if (y != 0 || y != height)
                            bTrees = y2 - y;

                        bSeen = false;
                        break;
                    }
                }

                if (bSeen)
                    bTrees = height - y - 1;

                if (lSeen || rSeen || tSeen || bSeen)
                    visCount += 1;

                var scenicScore = lTrees * rTrees * tTrees * bTrees;
                maxScore = scenicScore > maxScore ? scenicScore : maxScore;
            }
        }

        Console.WriteLine($"Visible: {visCount}");
        Console.WriteLine($"Max Scenic Score: {maxScore}");
    }

}