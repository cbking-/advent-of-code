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

//ToDo: run all days in year
//ToDo: run all days in all years

//want to keep new lines for 2022 day 1 since elves are separated
// by a new line
var keepNewLines = false;
if (args[0] == "2022" && args[1] == "Day1")
    keepNewLines = true;

Assembly.GetExecutingAssembly()
        .GetTypes()
        .Single(t => t.IsClass
            && t.Namespace == $"Advent{args[0]}"
            && t.Name == args[1])
        .GetMethod("Run", BindingFlags.Static | BindingFlags.Public)
       ?.Invoke(null, new object[] { await LoadDataAsync(args, keepNewLines) });
