namespace Advent2021;

public static class Day6
{
    public static void Run(string[] data)
    {
        //First solution was to create a list of integers
        // where each integer was a fish's countdown. Then
        // Add to the list once it reached zero and add one to 8 and
        // reset the current fish to 6.
        // This worked for the part one but exponential growth kills part two.
        // This solution keeps track of the number of fish in each day and the new fishes
        // that are generated. The growth is tracked through longs rather than
        // the size of a List. I keep track of new fish in a separate array
        // so they aren't overwritten as the other fish that haven't spawned
        // are figured out. I'm going to start using a time in the interest
        // of seeing how quick my solutions are and if certain implementations
        // are quicker than others.

        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        var fishes = new long[9];

        Array.ConvertAll(data[0].Split(','), int.Parse).ToList().ForEach(state => fishes[state] += 1);

        foreach (var day in Enumerable.Range(1, 80))
        {
            var nextFishes = new long[9];
            var nextNewFishes = new long[9];

            Array.Copy(fishes, nextFishes, fishes.Length);

            foreach (var group in fishes.WithIndex())
            {
                if (group.Index == 0)
                {
                    nextNewFishes[8] += fishes[0];
                    nextNewFishes[6] += fishes[0];
                    nextFishes[0] = 0;
                }
                else
                {
                    nextFishes[group.Index - 1] += fishes[group.Index];
                    nextFishes[group.Index] = nextNewFishes[group.Index]; //this will put in the sixes and eights
                }
            }

            Array.Copy(nextFishes, fishes, nextFishes.Length);
        }

        Console.WriteLine($"Part 1: \x1b[93m{fishes.Sum()}\x1b[0m");

        watch.Stop();
        Console.WriteLine($"Part 1 Execution Time: {watch.ElapsedMilliseconds} ms");

        watch.Reset();
        watch.Start();

        //continue to the 256th day
        foreach (var day in Enumerable.Range(1, 176))
        {
            var nextFishes = new long[9];
            var nextNewFishes = new long[9];

            Array.Copy(fishes, nextFishes, fishes.Length);

            foreach (var group in fishes.WithIndex())
            {
                if (group.Index == 0)
                {
                    nextNewFishes[8] += fishes[0];
                    nextNewFishes[6] += fishes[0];
                    nextFishes[0] = 0;
                }
                else
                {
                    nextFishes[group.Index - 1] += fishes[group.Index];
                    nextFishes[group.Index] = nextNewFishes[group.Index]; //this will put in the sixes and eights
                }
            }

            Array.Copy(nextFishes, fishes, nextFishes.Length);
        }

        Console.WriteLine($"Part 2: \x1b[93m{fishes.Sum()}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 2 Execution Time: {watch.ElapsedMilliseconds} ms");
    }

}