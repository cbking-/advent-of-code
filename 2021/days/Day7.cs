using MathNet.Numerics.Statistics;

namespace Advent2021;

public static class Day7
{
    public static void Run(string[] data)
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        //double since we will be caclulating the median which could be a double value
        // Though this is unlikely as AOC only deals with integers but that's how
        // the library implements it
        var positions = Array.ConvertAll(data[0].Split(',', StringSplitOptions.RemoveEmptyEntries), double.Parse);

        //added a whole package for getting the median but their implementation is going to be way better
        // than anything I would implement (copy + paste from stackoverflow)
        var positionToMoveTo = positions.Median();

        Console.WriteLine($"Part 1: \x1b[93m{positions.Aggregate(0, (acc, position) => (int)Math.Abs(position - positionToMoveTo) + acc)}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 1 Execution Time: {watch.ElapsedMilliseconds} ms");

        watch.Reset();
        watch.Start();
        var bestMovement = int.MaxValue;

        foreach (var tryPosition in Enumerable.Range(0, (int)positions.Max()))
        {
            bestMovement = Math.Min(bestMovement, positions.Aggregate(0, (acc, position) =>
            {
                var n = (int)Math.Abs(position - tryPosition);
                return ((n * (n + 1)) / 2) + acc;
            }));
        }

        Console.WriteLine($"Part 2: \x1b[93m{bestMovement}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 2 Execution Time: {watch.ElapsedMilliseconds} ms");
    }

}