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
        var elves = data.Aggregate(new List<List<int>> { new List<int>() }, (acc, line) =>
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                acc.Add(new List<int>());
            }
            else
            {
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
            switch (me)
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

    public static void Day3(string[] data)
    {
        var prioritySum = data.Aggregate(0, (acc, line) =>
        {
            var compartmentOne = line.Take(line.Length / 2);
            var compartmentTwo = line.Skip(line.Length / 2);

            var commonItem = compartmentOne.Intersect(compartmentTwo);

            var priority = (int)(char)commonItem.Single();

            if (priority >= 97 && priority <= 122)
                priority = priority - 96;
            else
                priority = priority - 38;

            return acc + priority;
        });

        var badgeSum = data.Batch(3).Aggregate(0, (acc, group) =>
        {
            var priority = (int)group.Skip(1).Aggregate(group.First(), (previous, current) =>
                string.Concat(previous.Intersect(current))
            ).Single();

            if (priority >= 97 && priority <= 122)
                priority = priority - 96;
            else
                priority = priority - 38;

            return acc + priority;
        });

        Console.WriteLine(prioritySum);
        Console.WriteLine(badgeSum);
    }

    public static void Day4(string[] data)
    {
        var overlapping = data.Aggregate(new int[]{0, 0}, (acc, line) =>{
            var elfOne = line.Split(',').First().Split('-').Select(Int32.Parse);
            var elfTwo = line.Split(',').Last().Split('-').Select(Int32.Parse);

            //completely overlapping
            if ((elfOne.First() <= elfTwo.First() && elfOne.Last() >= elfTwo.Last())
             || (elfTwo.First() <= elfOne.First() && elfTwo.Last() >= elfOne.Last()))
                 acc[0] += 1;

            //partially overlapping (is the end of one in the other's range)
            if ((elfOne.Last() >= elfTwo.First() && elfOne.Last() <= elfTwo.Last())
             || (elfTwo.Last() >= elfOne.First() && elfTwo.Last() <= elfOne.Last()))
                 acc[1] += 1;

            return acc;
        });

        Console.WriteLine(overlapping[0]);
        Console.WriteLine(overlapping[1]);
    }
}