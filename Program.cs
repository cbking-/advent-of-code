using System.Reflection;

var adventType = typeof(AdventOfCode);

var dayToRun = adventType.GetMethod(args[0], BindingFlags.Static | BindingFlags.Public) ?? throw new ArgumentException("Invalid day");

dynamic? task = dayToRun.Invoke(null, null);
await task;

public static class AdventOfCode
{
    public static async Task Day1()
    {
        Console.WriteLine("Hello");
    }

}
