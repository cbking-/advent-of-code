using System.Reflection;
using static Core.Helpers;

var data = await LoadDataAsync(args[0]);

var adventType = typeof(AdventOfCode);
var dayToRun = adventType.GetMethod(args[0], BindingFlags.Static | BindingFlags.Public) ?? throw new ArgumentException("Invalid day");
dayToRun.Invoke(null, new object[] { data });

public static class AdventOfCode
{
    #region Objects
    #endregion
    
    public static void Day1(string[] data)
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

    public static void Day2(string[] data)
    {
        //Initial solutions were using regex to parse and foreach loops Linq is
        // a little more elegant and Regex was a bit overkill

        //I don't think group by can be used here since
        // forward uses the current cumulative value of aim per iteration 
        var calculation = data.Aggregate((0, 0 ,0),
            (accumulator, line) => {
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
