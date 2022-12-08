using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using static Core.Helpers;

var keepNewLines = new bool[25];
keepNewLines[1] = true;

var data = await LoadDataAsync(args[0], keepNewLines[int.Parse(args[0].Substring(3))]);

var adventType = typeof(AdventOfCode);
var dayToRun = adventType.GetMethod(args[0], BindingFlags.Static | BindingFlags.Public) ?? throw new ArgumentException("Invalid day");
dayToRun.Invoke(null, new object[] { data });

public class AdventOfCode
{
    [DebuggerDisplay("{Name}: {Size}")]
    private class ElfDirectory
    {
        public string Name = string.Empty;

        public List<ElfFile> Files = new();

        public List<ElfDirectory>? Directories;

        public int Size = 0;

        public int GetSize()
        {
            if (Size == 0)
                Size = (Directories?.Sum(directory => directory.GetSize()) ?? 0) + Files.Sum(file => file.Size);

            return Size;
        }
    }

    private class ElfFile
    {
        public string Name = string.Empty;
        public int Size = 0;
    }

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
        var overlapping = data.Aggregate(new int[] { 0, 0 }, (acc, line) =>
        {
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

    public static void Day5(string[] data)
    {
        //    [D]
        //[N] [C]
        //[Z] [M] [P]
        // 1   2   3
        //
        //move 1 from 2 to 1
        //move 3 from 1 to 3
        //move 2 from 2 to 1
        //move 1 from 1 to 2

        var endInitialCreates = new Regex(@"^(\s*(\d+)\s*)+$");

        var splitIndex = data.WithIndex()
                             .Where(line => endInitialCreates.IsMatch(line.Item))
                             .Select(line => line.Index)
                             .Single();

        var splitData = data.WithIndex()
                            .GroupBy(line => line.Index > splitIndex)
                            .Select(group => group.Select(line => line.Item));

        var numberOfStacks = int.Parse(endInitialCreates.Matches(splitData.First().Last())
                                              .Last().Groups[2].Value);

        var stacks = new Stack[numberOfStacks].Select(stack => new Stack()).ToArray();
        var betterStacks = new Stack[numberOfStacks].Select(stack => new Stack()).ToArray();

        splitData.First().Reverse().Skip(1).ToList().ForEach(line =>
        {
            line.Skip(1)
                .WithIndex()
                .Where(crate => crate.Index % 4 == 0)
                .ToList()
                .ForEach(crate =>
                {
                    if (crate.Item != ' ')
                    {
                        stacks[crate.Index / 4].Push(crate.Item);
                        betterStacks[crate.Index / 4].Push(crate.Item);
                    }
                });
        });

        splitData.Last().ToList().ForEach(move =>
        {
            var movePattern = new Regex(@"move (\d+) from (\d+) to (\d+)");
            var moveData = movePattern.Matches(move);

            var numberOfCrates = int.Parse(moveData.Single().Groups[1].Value);
            var fromStack = int.Parse(moveData.Single().Groups[2].Value);
            var toStack = int.Parse(moveData.Single().Groups[3].Value);

            foreach (var index in Enumerable.Range(0, numberOfCrates))
            {
                stacks[toStack - 1].Push(stacks[fromStack - 1].Pop());
            }

            var interStack = new Stack();

            foreach (var index in Enumerable.Range(0, numberOfCrates))
            {
                interStack.Push(betterStacks[fromStack - 1].Pop());
            }

            foreach (var index in Enumerable.Range(0, numberOfCrates))
            {
                betterStacks[toStack - 1].Push(interStack.Pop());
            }
        });

        StringBuilder builder = new();

        foreach (var stack in stacks)
        {
            builder.Append(stack.Pop());
        }

        Console.WriteLine(builder.ToString());

        builder = new();

        foreach (var stack in betterStacks)
        {
            builder.Append(stack.Pop());
        }

        Console.WriteLine(builder.ToString());
    }

    public static void Day6(string[] data)
    {
        var line = data.First();

        for (var n = 0; n < line.Length; n++)
        {
            if (line.Skip(n).Take(4).Distinct().Count() == 4)
            {
                Console.WriteLine(n + 4);
                break;
            }
        }

        for (var n = 0; n < line.Length; n++)
        {
            if (line.Skip(n).Take(14).Distinct().Count() == 14)
            {
                Console.WriteLine(n + 14);
                break;
            }
        }
    }

    public static void Day7(string[] data)
    {
        List<string> path = new();
        List<ElfDirectory> fileSystem = new();
        ElfDirectory currentDir = new();

        foreach (var line in data)
        {
            var tokens = line.Split(' ');

            if (tokens.First() == "$")
            {
                var command = tokens.Skip(1).First();
                var argument = tokens.Skip(2).FirstOrDefault();

                if (command == "cd")
                {
                    if (argument == "..")
                    {
                        path.RemoveAt(path.Count - 1);
                    }
                    else
                    {
                        path.Add(argument ?? "");

                        currentDir = new ElfDirectory
                        {
                            Name = string.Join('/', path)
                        };
                    }
                }
                else
                {
                    //theres no processing for ls commands
                    continue;
                }
            }
            else
            {
                //Taking advantage of object references
                var existingDir = fileSystem.Find(dir => dir.Name == currentDir.Name);

                if (existingDir is not null)
                {
                    //The directory was found in a previous command
                    // and we are populating that directory now
                    currentDir = existingDir;
                }
                else
                {
                    fileSystem.Add(currentDir);
                }

                if (tokens.First() == "dir")
                {
                    if (currentDir.Directories is null)
                        currentDir.Directories = new();

                    //add to the path temporarily
                    path.Add(tokens.Last());

                    var newDir = new ElfDirectory
                    {
                        Name = string.Join('/', path)
                    };

                    path.RemoveAt(path.Count - 1);

                    fileSystem.Add(newDir);
                    currentDir.Directories.Add(newDir);
                }
                else
                {
                    currentDir.Files.Add(new ElfFile
                    {
                        Name = tokens.Last(),
                        Size = int.Parse(tokens.First())
                    });
                }
            }
        }

        var sum = fileSystem.Where(dir => dir.GetSize() <= 100000)
                            .Sum(dir => dir.Size);

        Console.WriteLine(sum);

        var freeSpace = 70000000 - fileSystem.First().GetSize();
        var spaceToClear = 30000000 - freeSpace;

        var dirToDelete = fileSystem.Where(dir => dir.Size >= spaceToClear).Min(dir => dir.Size);

        Console.WriteLine(dirToDelete);
    }
}