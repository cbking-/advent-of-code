namespace Advent2021;

public static class Day13
{
    public static void Run(string[] data)
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        var dots = new HashSet<int[]>();
        var folds = new List<string>();

        foreach (var line in data)
        {
            if (line.IndexOf(',') > -1)
            {
                dots.Add(new int[] { int.Parse(line.Split(',')[0]), int.Parse(line.Split(',')[1]) });
            }
            else
            {
                folds.Add(line.Split(' ')[2]);
            }
        }

        var width = dots.Max(set => set[0]) + 1;
        var height = dots.Max(set => set[1]) + 1;

        var dotArray = Enumerable.Repeat('\x2591', width * height).ToArray();

        foreach (var dot in dots)
        {
            dotArray[dot[0] + dot[1] * width] = '\x2588';
        }

        foreach (var fold in folds.Take(1))
        {
            var split = int.Parse(fold.Split('=')[1]);

            if (fold.Split('=')[0] == "y")
            {
                foreach (var y in Enumerable.Range(split, height - split))
                {
                    foreach (var x in Enumerable.Range(0, width))
                    {
                        var newY = Math.Abs((y - split) - split);

                        if (dotArray[x + newY * width] != '\x2588' && dotArray[x + y * width] == '\x2588')
                        {
                            dotArray[x + newY * width] = '\x2588';
                        }

                        dotArray[x + y * width] = ' ';
                    }
                }
            }

            if (fold.Split('=')[0] == "x")
            {
                foreach (var y in Enumerable.Range(0, height))
                {
                    foreach (var x in Enumerable.Range(split, width - split))
                    {
                        var newX = Math.Abs((x - split) - split);

                        if (dotArray[newX + y * width] != '\x2588' && dotArray[x + y * width] == '\x2588')
                        {
                            dotArray[newX + y * width] = '\x2588';
                        }

                        dotArray[x + y * width] = ' ';
                    }
                }
            }
        }

        Console.WriteLine($"Part 1: \x1b[93m{dotArray.Count(dot => dot == '\x2588')}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 1 Execution Time: {watch.ElapsedMilliseconds} ms");

        watch.Reset();
        watch.Start();

        foreach (var fold in folds.Skip(1))
        {
            var split = int.Parse(fold.Split('=')[1]);

            if (fold.Split('=')[0] == "y")
            {
                foreach (var y in Enumerable.Range(split, height - split))
                {
                    foreach (var x in Enumerable.Range(0, width))
                    {
                        var newY = Math.Abs((y - split) - split);

                        if (dotArray[x + newY * width] != '\x2588' && dotArray[x + y * width] == '\x2588')
                        {
                            dotArray[x + newY * width] = '\x2588';
                        }

                        dotArray[x + y * width] = ' ';
                    }
                }
            }

            if (fold.Split('=')[0] == "x")
            {
                foreach (var y in Enumerable.Range(0, height))
                {
                    foreach (var x in Enumerable.Range(split, width - split))
                    {
                        var newX = Math.Abs((x - split) - split);

                        if (dotArray[newX + y * width] != '\x2588' && dotArray[x + y * width] == '\x2588')
                        {
                            dotArray[newX + y * width] = '\x2588';
                        }

                        dotArray[x + y * width] = ' ';
                    }
                }
            }
        }

        Console.WriteLine($"Part 2: \x1b[93m");
        foreach (var y in Enumerable.Range(0, height))
        {
            foreach (var x in Enumerable.Range(0, width))
            {
                if (dotArray[x + y * width] != ' ')
                    Console.Write(dotArray[x + y * width]);
            }

            if (dotArray[0 + y * width] != ' ')
                Console.Write(Environment.NewLine);
        }
        Console.WriteLine("\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 2 Execution Time: {watch.ElapsedMilliseconds} ms");
    }

}