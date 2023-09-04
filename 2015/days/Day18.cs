namespace Advent2015;

public static class Day18
{
    public static void Run(string[] data)
    {
        var width = data.First().Length;

        var grid = Enumerable.Repeat(false, width * width).ToArray();

        foreach (var line in data.WithIndex())
        {
            foreach (var character in line.Item.WithIndex())
            {
                grid[character.Index + line.Index * width] = character.Item == '#';
            }
        }

        foreach (var step in Enumerable.Range(0, 100))
        {
            var gridCopy = Enumerable.Repeat(false, width * width).ToArray();
            Array.Copy(grid, gridCopy, grid.Length);

            foreach (var y in Enumerable.Range(0, width))
            {
                foreach (var x in Enumerable.Range(0, width))
                {
                    //constraining neighbors to grid
                    var xMinus = x - 1 >= 0 ? x - 1 : -width * width;
                    var xPlus = x + 1 < width ? x + 1 : -width * width;
                    var yMinus = y - 1 >= 0 ? y - 1 : -width * width;
                    var yPlus = y + 1 < width ? y + 1 : -width * width;

                    var neighbors = new bool[]{
                        grid.ElementAtOrDefault(xMinus + yMinus * width), //top left
                        grid.ElementAtOrDefault(x      + yMinus * width), //top mid
                        grid.ElementAtOrDefault(xPlus  + yMinus * width), //top right
                        grid.ElementAtOrDefault(xMinus + y      * width), //mid left
                        grid.ElementAtOrDefault(xPlus  + y      * width), //mid right
                        grid.ElementAtOrDefault(xMinus + yPlus  * width), //bot left
                        grid.ElementAtOrDefault(x      + yPlus  * width), //bot mid
                        grid.ElementAtOrDefault(xPlus  + yPlus  * width), //bot right
                    };

                    var location = x + y * width;

                    var state = grid[location];

                    if (state && neighbors.Count(light => light) != 2 && neighbors.Count(light => light) != 3)
                    {
                        gridCopy[location] = false;
                    }

                    if (!state && neighbors.Count(light => light) == 3)
                    {
                        gridCopy[location] = true;
                    }
                }
            }

            grid = gridCopy;
        }

        Console.WriteLine($"Part 1: {grid.Count(light => light)}");

        grid = Enumerable.Repeat(false, width * width).ToArray();

        foreach (var line in data.WithIndex())
        {
            foreach (var character in line.Item.WithIndex())
            {
                grid[character.Index + line.Index * width] = character.Item == '#';
            }
        }

        foreach (var step in Enumerable.Range(0, 100))
        {
            grid[0] = true; //top left
            grid[width - 1] = true; //top right
            grid[(width - 1) * width] = true; //bottom left
            grid[(width - 1) + (width - 1) * width] = true; //bottom right

            var gridCopy = Enumerable.Repeat(false, width * width).ToArray();
            Array.Copy(grid, gridCopy, grid.Length);

            foreach (var y in Enumerable.Range(0, width))
            {
                foreach (var x in Enumerable.Range(0, width))
                {
                    //constraining neighbors to grid
                    var xMinus = x - 1 >= 0 ? x - 1 : -width * width;
                    var xPlus = x + 1 < width ? x + 1 : -width * width;
                    var yMinus = y - 1 >= 0 ? y - 1 : -width * width;
                    var yPlus = y + 1 < width ? y + 1 : -width * width;

                    var neighbors = new bool[]{
                        grid.ElementAtOrDefault(xMinus + yMinus * width), //top left
                        grid.ElementAtOrDefault(x      + yMinus * width), //top mid
                        grid.ElementAtOrDefault(xPlus  + yMinus * width), //top right
                        grid.ElementAtOrDefault(xMinus + y      * width), //mid left
                        grid.ElementAtOrDefault(xPlus  + y      * width), //mid right
                        grid.ElementAtOrDefault(xMinus + yPlus  * width), //bot left
                        grid.ElementAtOrDefault(x      + yPlus  * width), //bot mid
                        grid.ElementAtOrDefault(xPlus  + yPlus  * width), //bot right
                    };

                    var location = x + y * width;

                    var state = grid[location];

                    if (state && neighbors.Count(light => light) != 2 && neighbors.Count(light => light) != 3)
                    {
                        gridCopy[location] = false;
                    }

                    if (!state && neighbors.Count(light => light) == 3)
                    {
                        gridCopy[location] = true;
                    }
                }
            }

            gridCopy[0] = true; //top left
            gridCopy[width - 1] = true; //top right
            gridCopy[(width - 1) * width] = true; //bottom left
            gridCopy[(width - 1) + (width - 1) * width] = true; //bottom right

            grid = gridCopy;

            // var splitGrid = grid.Select((item, index) => new { Index = index, Value = item })
            //                     .GroupBy(item => item.Index / width)
            //                     .Select(item => item.Select(v => v.Value).ToList())
            //                     .ToList();

            // foreach (var line in splitGrid)
            // {
            //     foreach (var light in line)
            //     {
            //         Console.Write(light ? "#" : ".");
            //     }
            //     Console.Write("\n");
            // }
            // Console.Write("\n==================\n");
        }

        Console.WriteLine($"Part 2: {grid.Count(light => light)}");
    }

}