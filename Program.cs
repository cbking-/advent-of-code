using System.Reflection;

var adventType = typeof(AdventOfCode);

var dataToLoad = adventType.GetMethod("LoadDataAsync", BindingFlags.Static | BindingFlags.Public) ?? throw new ArgumentException("Method not found");
dynamic? loadTask = dataToLoad.Invoke(null, new object[] { $"inputs{Path.DirectorySeparatorChar}{args[0]}.txt" });
var data = await loadTask;

var dayToRun = adventType.GetMethod(args[0], BindingFlags.Static | BindingFlags.Public) ?? throw new ArgumentException("Invalid day");
dayToRun.Invoke(null, new object[] { data });

public static class AdventOfCode
{
    #region Helpers

    public static async Task<string[]> LoadDataAsync(string fileName)
    {
        using var file = new StreamReader(fileName);
        var data = await file.ReadToEndAsync();
        return data.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
    }

    public static IEnumerable<(T Item, int Index)> WithIndex<T>(this IEnumerable<T> source)
    {
        return source.Select((Item, Index) => (Item, Index));
    }

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

}
