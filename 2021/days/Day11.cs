namespace Advent2021;

public static class Day11
{
    public static void Run(string[] data)
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        //converting 1d array to 2d array because that's how I roll
        var width = data[0].Length;
        var octopi = string.Join("", data).ToCharArray();
        var flashCount = 0;
        var flashed = new List<int>();

        Action Draw = () =>
        {
            foreach (var y in Enumerable.Range(0, width))
            {
                foreach (var x in Enumerable.Range(0, width))
                {
                    if (octopi[x + y * width] == '0')
                    {
                        Console.Write($"\x1b[93m{octopi[x + y * width]}\x1b[0m");
                    }
                    else
                    {
                        Console.Write(octopi[x + y * width]);
                    }

                }
                Console.Write(Environment.NewLine);
            }
            Console.WriteLine("=================");
        };

        Action<int> Flash = (index) => { };

        Flash = (index) =>
        {
            flashCount += 1;
            flashed.Add(index);

            var left = index % width == 0 ? -1 : index - 1;
            var right = (index + 1) % width == 0 ? -1 : index + 1;
            var bottom = (index + width) >= octopi.Length ? -1 : index + width;
            var bottomLeft = left == -1 || (left + width) >= octopi.Length ? -1 : left + width;
            var bottomRight = right == -1 || (right + width) >= octopi.Length ? -1 : right + width;

            var neighbors = new int[]{
                index - width,
                left,
                right,
                bottom,
                left - width,
                right - width,
                bottomLeft,
                bottomRight
            };

            neighbors = neighbors.Where(neighborIndex => neighborIndex >= 0 && !flashed.Contains(neighborIndex)).ToArray();

            foreach (var neighborIndex in neighbors)
            {
                if (!flashed.Contains(neighborIndex))
                    octopi[neighborIndex] += (char)1;
            }

            foreach (var neighborIndex in neighbors)
            {
                if (octopi[neighborIndex] < ':')
                    continue;

                Flash(neighborIndex);

                octopi[neighborIndex] = '0';
            }
        };

        foreach (var step in Enumerable.Range(0, 100))
        {
            flashed = new List<int>();

            for (var index = 0; index < octopi.Length; index++)
            {
                //converting the array to ints is not necessary
                // as chars are pretty much ints already
                if (!flashed.Contains(index))
                    octopi[index] += (char)1;

                if (octopi[index] < ':')
                    continue;

                Flash(index);

                octopi[index] = '0';
            }
            //Draw();
        }

        Console.WriteLine($"Part 1: \x1b[93m{flashCount}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 1 Execution Time: {watch.ElapsedMilliseconds} ms");

        watch.Reset();
        watch.Start();

        //we've already completed 100 steps so keep stepping until
        // the flashed count is equal to the number of octopi
        var syncStep = 100;

        while (flashed.Count != octopi.Length)
        {
            flashed = new List<int>();

            for (var index = 0; index < octopi.Length; index++)
            {
                if (!flashed.Contains(index))
                    octopi[index] += (char)1;

                if (octopi[index] < ':')
                    continue;

                Flash(index);

                octopi[index] = '0';
            }

            syncStep += 1;
        }

        Console.WriteLine($"Part 1: \x1b[93m{syncStep}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 2 Execution Time: {watch.ElapsedMilliseconds} ms");
    }

}