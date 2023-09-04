namespace Advent2021;

public static class Day2
{
    public static void Run(string[] data)
    {
        //Initial solutions were using regex to parse and foreach loops Linq is
        // a little more elegant and Regex was a bit overkill

        //I don't think group by can be used here since
        // forward uses the current cumulative value of aim per iteration
        var calculation = data.Aggregate((0, 0, 0),
            (accumulator, line) =>
            {
                var split = line.Split(' ');

                if (split[0] == "down")
                {
                    accumulator.Item1 += int.Parse(split[1]);
                }

                if (split[0] == "up")
                {
                    accumulator.Item1 -= int.Parse(split[1]);
                }

                if (split[0] == "forward")
                {
                    accumulator.Item2 += int.Parse(split[1]);
                    accumulator.Item3 += accumulator.Item1 * int.Parse(split[1]);
                }

                return accumulator;
            });

        Console.WriteLine($"Part 1: {calculation.Item2 * calculation.Item1}");
        Console.WriteLine($"Part 2: {calculation.Item2 * calculation.Item3}");
    }

}