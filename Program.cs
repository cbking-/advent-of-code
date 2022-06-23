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
        var prevNum = int.Parse(data[0]);

        var part1 = 0;

        foreach(var line in data.Skip(1))
        {
            if (prevNum < int.Parse(line))
            {
                part1++;
            }

            prevNum = int.Parse(line);
        }

        Console.WriteLine($"Part 1: {part1}");

        var prevSum = data.Take(3).Select(line => int.Parse(line)).Sum();
        var part2 = 0;
        foreach(var line in data.WithIndex().Skip(1))
        {
            var sum = data.Skip(line.Index).Take(3).Select(line => int.Parse(line)).Sum();

            if(prevSum < sum){
                part2++;
            }

            prevSum = sum;
        }

        Console.WriteLine($"Part 2: {part2}");
    }

}
