using System.Reflection;
using static Core.Helpers;

var keepNewLines = new bool[25];
keepNewLines[1] = true;

var data = await LoadDataAsync(args[0], keepNewLines[int.Parse(args[0].Substring(3))]);

var adventType = typeof(AdventOfCode);
var dayToRun = adventType.GetMethod(args[0], BindingFlags.Static | BindingFlags.Public) ?? throw new ArgumentException("Invalid day");
dayToRun.Invoke(null, new object[] { data });

public class AdventOfCode
{
    public static void Day1(string[] data)
    {
        var elves = data.Aggregate(new List<List<int>>{new List<int>()}, (acc, line) =>
        {
            if(string.IsNullOrWhiteSpace(line)) {
                acc.Add(new List<int>());
            }
            else{
                acc.Last().Add(int.Parse(line));
            }

            return acc;
        });

        Console.WriteLine(elves.Select(elf => elf.Sum()).Max());
        Console.WriteLine(elves.Select(elf => elf.Sum()).OrderByDescending(total => total).Take(3).Sum());
    }
}