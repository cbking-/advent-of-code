namespace Advent2021;

public static class Day1
{
    public static void Run(string[] data)
    {
        var numbers = Array.ConvertAll(data, line => int.Parse(line));

        //skip the first one since there's nothing to compare it to
        // Calling Skip(x) will be zero indexed (creates a new list) but pass that index to the
        // data list to get the previous value from the current one from the Skip list.
        // Generates list of true or false then count the true values.
        Console.WriteLine($"Part 1: {numbers.Skip(1).Select((value, index) => value > numbers[index]).Count(value => value)}");

        //skip the first three since there's nothing to compare them to
        // since two windows share two of the same numbers, we don't need to calculate any sums
        // only compare the first number of the first window and the last number of the second window
        Console.WriteLine($"Part 2: {numbers.Skip(3).Select((value, index) => value > numbers[index]).Count(value => value)}");

        //Another option (and maybe makes more sense) is to use Zip as we don't rely on knowing Skip is indexed zero
        // Console.WriteLine($"Part 1: {numbers.Zip(numbers.Skip(1), (first, second) => first < second).Count(value => value)}");
        // Console.WriteLine($"Part 2: {numbers.Zip(numbers.Skip(3), (first, second) => first < second).Count(value => value)}");

        //Aggregate is an option here as well though I don't like it as much as Zip
        // Console.WriteLine($@"Part 1: {numbers.Skip(1).Select((Value, Index) => new {Value, Index})
        //                                              .Aggregate(0, (total, line) => numbers[line.Index] < line.Value ? total += 1 : total)}");
        // Console.WriteLine($@"Part 2: {numbers.Skip(3).Select((Value, Index) => new {Value, Index})
        //                                              .Aggregate(0, (total, line) => numbers[line.Index] < line.Value ? total += 1 : total)}");
    }

}