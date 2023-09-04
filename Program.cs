global using static Core.Helpers;
global using Core;
global using System.Text;
global using System.Text.RegularExpressions;
global using Newtonsoft.Json;
global using Newtonsoft.Json.Linq;
global using Combinatorics.Collections;
global using System.Collections.Concurrent;
global using System.Security.Cryptography;
using System.Reflection;

//ToDo: run all days in all years

//want to keep new lines for 2022 day 1 since elves are separated
// by a new line

if (args[1] == "All")
{
    List<Type> types = Assembly.GetExecutingAssembly()
        .GetTypes()
        .Where(t => t.IsClass && t.Namespace == $"Advent{args[0]}" && t.Name.Contains("Day"))
        .OrderBy(t => int.Parse(t.Name[3..]))
        .ToList();

    foreach (Type t in types)
    {
        Console.WriteLine();
        Console.WriteLine(t.Name);
        t.GetMethod("Run", BindingFlags.Static | BindingFlags.Public)
        ?.Invoke(null, new object[] {
            LoadDataAsync(new string[]{args[0], t.Name}
                , args[0] == "2022" && t.Name == "Day1").Result
        });
    }
}
else
{
    Assembly.GetExecutingAssembly()
            .GetTypes()
            .Single(t => t.IsClass
                && t.Namespace == $"Advent{args[0]}"
                && t.Name == args[1])
            .GetMethod("Run", BindingFlags.Static | BindingFlags.Public)
           ?.Invoke(null, new object[] { await LoadDataAsync(args, args[0] == "2022" && args[1] == "Day1") });
}
