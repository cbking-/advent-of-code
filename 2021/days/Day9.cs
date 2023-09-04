namespace Advent2021;

public static class Day9
{
    public static void Run(string[] data)
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        //looks weird but join all lines from the data then split into individual characters.
        // I like interacting with 2d arrays as a 1d array and using the x + y * width formula
        var width = data[0].Length;
        var map = string.Join("", data);

        var lowPoints = map.WithIndex().Select(point =>
        {
            var neighbors = new char[]{
                map.ElementAtOrDefault(point.Index % width == 0 ? -1 : point.Index - 1),     //left neighbor
                map.ElementAtOrDefault((point.Index + 1) % width == 0 ? -1 : point.Index + 1),     //right neighbor
                map.ElementAtOrDefault(point.Index - width), //top neighbor
                map.ElementAtOrDefault(point.Index + width)  //bottom neighbor
            };

            if (point.Item < neighbors.Where(character => character != 0).Min())
            {
                return point.Index;
            }

            return -1;
        }).Where(point => point != -1);

        var partOne = lowPoints.Aggregate(0, (acc, point) => int.Parse(map.ElementAtOrDefault(point).ToString()) + 1 + acc);

        Console.WriteLine($"Part 1: \x1b[93m{partOne}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 1 Execution Time: {watch.ElapsedMilliseconds} ms");

        watch.Reset();
        watch.Start();

        var partTwo = lowPoints.Select(point =>
        {
            var basin = new List<int>{
                point,
                point % width == 0 ? -1 : point - 1,
                (point + 1) % width == 0 ? -1 : point + 1,
                point - width,
                point + width
            };

            basin = basin.Where(location => map.ElementAtOrDefault(location) != '9' && map.ElementAtOrDefault(location) != 0).ToList();

            var searchIndexes = basin.AsEnumerable(); ;

            do
            {
                var initialIndexes = basin.AsEnumerable();

                var addToBasin = searchIndexes.SelectMany(index => new List<int>{
                                                                            index % width == 0 ? -1 : index - 1,
                                                                            (index + 1) % width == 0 ? -1 : index + 1,
                                                                            index - width,
                                                                            index + width
                                                                        })
                                              .Where(location => map.ElementAtOrDefault(location) != '9' && map.ElementAtOrDefault(location) != 0);

                basin = basin.Union(addToBasin).ToList();

                searchIndexes = basin.Except(initialIndexes);

            } while (searchIndexes.Count() > 0);

            return basin.Count();
        }).OrderByDescending(basin => basin).Take(3).Aggregate(1, (acc, basin) => basin * acc);

        Console.WriteLine($"Part 2: \x1b[93m{partTwo}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 2 Execution Time: {watch.ElapsedMilliseconds} ms");
    }

}