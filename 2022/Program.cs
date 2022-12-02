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

    public static void Day2(string[] data)
    {
        const string LOSE = "X";
        const string DRAW = "Y";
        const string WIN = "Z";

        var states = new Dictionary<string, int>{
            {"A X", 4},
            {"A Y", 8},
            {"A Z", 3},

            {"B X", 1},
            {"B Y", 5},
            {"B Z", 9},

            {"C X", 7},
            {"C Y", 2},
            {"C Z", 6}
        };

        var score = data.Aggregate(0, (acc, line) =>
        {
            var score = states[line];
            return acc + score;
        });

        var scorePart2 = data.Aggregate(0, (acc, line) =>
        {
            var match = line.Split(' ');

            var elf = match.First().ToCharArray().First().ToString();
            var me = match.Last().ToCharArray().First().ToString();

            var score = 0;
            switch(me)
            {
                case WIN:
                    score = states.Where(state => state.Key.StartsWith(elf))
                        .Max(state => state.Value);
                    break;

                case LOSE:
                    score = states.Where(state => state.Key.StartsWith(elf))
                            .Min(state => state.Value);
                    break;

                case DRAW:
                    score = states.Where(state => state.Key.StartsWith(elf))
                            .OrderBy(state => state.Value).Skip(1).First().Value;
                    break;
            }

            return acc + score;
        });

        Console.WriteLine(score);
        Console.WriteLine(scorePart2);
    }

}