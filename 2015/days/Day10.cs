namespace Advent2015;

public static class Day10
{
    public static void Run(string[] data)
    {
        var iteration = data[0];

        var pattern = @"((\d)\2{0,})+";

        for (var step = 1; step <= 40; step++)
        {
            var captures = Regex.Match(iteration, pattern).Groups[1].Captures;

            iteration = string.Concat(captures.Select(capture => capture.Length.ToString() + capture.Value.Last()));
        }

        Console.WriteLine($"Part 1: {iteration.Length}");

        iteration = data[0];

        for (var step = 1; step <= 50; step++)
        {
            var captures = Regex.Match(iteration, pattern).Groups[1].Captures;

            iteration = string.Concat(captures.Select(capture => capture.Length.ToString() + capture.Value.Last()));
        }

        Console.WriteLine($"Part 1: {iteration.Length}");
    }
}