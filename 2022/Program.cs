using System.Collections;
using System.Data;
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

    private class Knot
    {
        public Knot()
        {
            //instead of figuring out how to deal with the head and tail being -1 and +1
            // i'll just set the initial coordinates way out there so it's incredibly
            // unlikely negatives even come in to consideration.
            X = int.MaxValue / 2;
            Y = int.MaxValue / 2;

            Visited.Add($"{X},{Y}");
        }

        public int X;
        public int Y;
        public Knot? Child;
        public HashSet<string> Visited = new();
        public char Label = '0';
        public Knot GetTail()
        {
            if (Child is not null)
            {
                return Child.GetTail();
            }

            return this;
        }

        public void Draw()
        {
            var grid = Enumerable.Repeat('.', 30 * 30).ToArray();
            var knot = this;

            while (knot is not null)
            {
                grid[knot.X + knot.Y * 30] = knot.Label;
                knot = knot.Child;
            }

            Console.WriteLine(string.Join('\n', grid.Chunk(30).Reverse().Select(chunck => string.Join("", chunck))));
            Console.WriteLine("=================");
        }

        public void Move(string direction, int steps)
        {
            foreach (var step in Enumerable.Range(1, steps))
            {
                switch (direction)
                {
                    case "U":
                        Y += 1;
                        break;

                    case "R":
                        X += 1;
                        break;

                    case "D":
                        Y -= 1;
                        break;

                    case "L":
                        X -= 1;
                        break;
                }

                MoveChildren();

                //Draw();
            }
        }

        public void MoveChildren()
        {
            if (Child is null)
                return;

            if (Child.X - X == 2)
            {
                Child.X -= 1;

                if (Child.Y - Y > 0)
                    Child.Y -= 1;

                if (Child.Y - Y < 0)
                    Child.Y += 1;
            }
            else if (Child.X - X == -2)
            {
                Child.X += 1;

                if (Child.Y - Y > 0)
                    Child.Y -= 1;

                if (Child.Y - Y < 0)
                    Child.Y += 1;
            }
            else if (Child.Y - Y == 2)
            {
                Child.Y -= 1;

                if (Child.X - X > 0)
                    Child.X -= 1;

                if (Child.X - X < 0)
                    Child.X += 1;
            }
            else if (Child.Y - Y == -2)
            {
                Child.Y += 1;

                if (Child.X - X > 0)
                    Child.X -= 1;

                if (Child.X - X < 0)
                    Child.X += 1;
            }


            Child.Visited.Add($"{Child.X},{Child.Y}");
            Child.MoveChildren();
        }
    }

    private class Monkey
    {
        public Queue<long> Items = new();

        public long Inspects = 0;

        public Func<long, long> Inspect = (long item) => { return 0; };

        public Func<long, long> WorseInspect = (long item) => { return 0; };

        public Func<long, int> ThrowTo = (long item) => {return 0;};
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
        Stack<string> path = new();
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
                        path.Pop();
                    }
                    else
                    {
                        path.Push(argument ?? "");

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
                    path.Push(tokens.Last());

                    var newDir = new ElfDirectory
                    {
                        Name = string.Join('/', path)
                    };

                    path.Pop();

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

    public static void Day8(string[] data)
    {
        var width = data[0].Length;
        var height = data.Length;
        var map = string.Join("", data);

        var visCount = 0;
        var maxScore = 0;

        foreach (var y in Enumerable.Range(0, height))
        {
            foreach (var x in Enumerable.Range(0, width))
            {
                var index = x + y * width;

                var tree = int.Parse(map[index].ToString());

                //check if any trees from here to edges are shorter
                var lSeen = true;
                var rSeen = true;
                var tSeen = true;
                var bSeen = true;

                var lTrees = 0;
                var rTrees = 0;
                var tTrees = 0;
                var bTrees = 0;

                //left edge
                foreach (var x2 in Enumerable.Range(0, x).Reverse())
                {
                    var otherIndex = x2 + y * width;
                    var otherTree = int.Parse(map[otherIndex].ToString());
                    if (otherTree >= tree)
                    {
                        if (x != 0 || x != width)
                            lTrees = x - x2;

                        lSeen = false;
                        break;
                    }
                }

                if (lSeen)
                    lTrees = x;

                //right edge
                foreach (var x2 in Enumerable.Range(x + 1, width - x - 1))
                {
                    var otherIndex = x2 + y * width;
                    var otherTree = int.Parse(map[otherIndex].ToString());
                    if (otherTree >= tree)
                    {
                        if (x != 0 || x != width)
                            rTrees = x2 - x;

                        rSeen = false;
                        break;
                    }
                }

                if (rSeen)
                    rTrees = width - x - 1;

                //top edge
                foreach (var y2 in Enumerable.Range(0, y).Reverse())
                {
                    var otherIndex = x + y2 * width;
                    var otherTree = int.Parse(map[otherIndex].ToString());
                    if (otherTree >= tree)
                    {
                        if (y != 0 || y != height)
                            tTrees = y - y2;

                        tSeen = false;
                        break;
                    }
                }

                if (tSeen)
                    tTrees = y;

                //bottom edge
                foreach (var y2 in Enumerable.Range(y + 1, height - y - 1))
                {
                    var otherIndex = x + y2 * width;
                    var otherTree = int.Parse(map[otherIndex].ToString());
                    if (otherTree >= tree)
                    {
                        if (y != 0 || y != height)
                            bTrees = y2 - y;

                        bSeen = false;
                        break;
                    }
                }

                if (bSeen)
                    bTrees = height - y - 1;

                if (lSeen || rSeen || tSeen || bSeen)
                    visCount += 1;

                var scenicScore = lTrees * rTrees * tTrees * bTrees;
                maxScore = scenicScore > maxScore ? scenicScore : maxScore;
            }
        }

        Console.WriteLine($"Visible: {visCount}");
        Console.WriteLine($"Max Scenic Score: {maxScore}");
    }

    public static void Day9(string[] data)
    {
        var head = new Knot();
        head.Label = 'H';
        var width = 40;

        Action Draw = () =>
        {
            var grid = Enumerable.Repeat('.', width * width).ToArray();
            var knot = head;

            while (knot is not null)
            {
                grid[knot.X + knot.Y * width] = knot.Label;
                knot = knot.Child;
            }

            Console.WriteLine(string.Join('\n', grid.Chunk(width).Reverse().Select(chunck => string.Join("", chunck))));
            Console.WriteLine("=================");
        };


        foreach (var index in Enumerable.Range(1, 9))
        {
            var child = new Knot();
            child.Label = char.Parse(index.ToString());
            head.GetTail().Child = child;
        }

        foreach (var line in data)
        {
            var instruction = line.Split(' ');
            var direction = instruction[0];
            var steps = int.Parse(instruction[1]);

            head.Move(direction, steps);

            //Draw();
        }

        //Console.WriteLine(string.Join('\n', head.GetTail().Visited));

        Console.WriteLine(head.Child?.Visited.Count);
        Console.WriteLine(head.GetTail().Visited.Count);
    }

    public static void Day10(string[] data)
    {
        int[] cycleChecks = {20, 60, 100, 140, 180, 220};
        int[] signalStrenths = {0, 0, 0, 0, 0, 0};

        var cycleNumber = 0;
        var x = 1;
        var pixelIndex = 0;

        Action IncrementCycle = () =>{
            if ((x + 1) == pixelIndex || (x - 1) == pixelIndex || x == pixelIndex)
            {
                Console.Write("\x2588");
            }
            else
            {
                Console.Write("\x2591");
            }

            pixelIndex += 1;
            cycleNumber += 1;

            if(cycleNumber % 40 == 0)
            {
                pixelIndex = 0;
                Console.Write(Environment.NewLine);
            }

            if(cycleChecks.Contains(cycleNumber))
            {
                var signalIndex = Array.IndexOf(cycleChecks, cycleNumber);
                signalStrenths[signalIndex] = cycleNumber * x;
            }

        };

        foreach(var line in data)
        {
            var instruction = line.Split(' ');
            var command = instruction[0];
            int.TryParse(instruction.ElementAtOrDefault(1), out int argument);

            IncrementCycle();

            if(command == "addx")
            {
                IncrementCycle();
                x += argument;
            }
        }

        Console.WriteLine(signalStrenths.Sum());
    }

    public static void Day11(string[] data)
    {
        List<Monkey> monkeys = new();

        //Had to look at solutions for this one
        //I don't completely understand the concept at work here
        var modulus = data.Chunk(6).Select(chunck => chunck.Where((item, index) => index % 3 == 0).Last().Split(' ', StringSplitOptions.RemoveEmptyEntries).Last())
                          .Aggregate(1, (acc, item) => int.Parse(item) * acc);

        Action SetUp = () => {
            monkeys = new();
            foreach(var monkeyData in data.Chunk(6))
        {
            var monkey = new Monkey();
            var items = monkeyData.ElementAt(1).Substring(17).Split(", ", StringSplitOptions.RemoveEmptyEntries);
            monkey.Items = new Queue<long>(items.Select(item => long.Parse(item)));

            var calculation = monkeyData.ElementAt(2).Split('=').Last().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var operation = calculation.Skip(1).First();
            var rightOperand = calculation.Last();

            monkey.Inspect = (long item) =>
            {
                if(rightOperand == "old")
                    item = Convert.ToInt64(new DataTable().Compute($"{item} {operation} {item}", null));
                else
                    item = Convert.ToInt64(new DataTable().Compute($"{item} {operation} {rightOperand}", null));

                return Convert.ToInt64(Math.Floor(item / 3.0));
            };

            monkey.WorseInspect = (long item) =>
            {
                if (rightOperand == "old")
                    item = Convert.ToInt64(new DataTable().Compute($"{item}.0 {operation} {item}.0", null));
                else
                    item = Convert.ToInt64(new DataTable().Compute($"{item}.0 {operation} {rightOperand}.0", null));

                return item % modulus;
            };

            var divisor = long.Parse(monkeyData.ElementAt(3).Split(' ').Last());
            var trueReturn = int.Parse(monkeyData.ElementAt(4).Split(' ').Last());
            var falseReturn = int.Parse(monkeyData.ElementAt(5).Split(' ').Last());

            monkey.ThrowTo = (long item) =>
            {
                if(item % divisor == 0)
                    return trueReturn;

                return falseReturn;
            };

            monkeys.Add(monkey);
        }
        };

        SetUp();

        foreach (var round in Enumerable.Range(1, 20))
        {
            foreach (var monkey in monkeys)
            {
                while (monkey.Items.Any())
                {
                    var item = monkey.Items.Dequeue();

                    item = monkey.Inspect(item);

                    var monkeyToThrowTo = monkey.ThrowTo(item);

                    monkeys.ElementAt(monkeyToThrowTo).Items.Enqueue(item);

                    monkey.Inspects += 1;
                }
            }
        }

        Console.WriteLine(monkeys.OrderByDescending(monkey => monkey.Inspects)
                                 .Take(2)
                                 .Select(monkey => monkey.Inspects)
                                 .Aggregate((long)1, (num, acc) =>  num * acc));

        SetUp();

        foreach (var round in Enumerable.Range(1, 10000))
        {
            foreach (var monkey in monkeys)
            {
                while (monkey.Items.Any())
                {
                    var item = monkey.Items.Dequeue();

                    item = monkey.WorseInspect(item);

                    var monkeyToThrowTo = monkey.ThrowTo(item);

                    monkeys.ElementAt(monkeyToThrowTo).Items.Enqueue(item);

                    monkey.Inspects += 1;
                }
            }
        }

        Console.WriteLine(monkeys.OrderByDescending(monkey => monkey.Inspects)
                                 .Take(2)
                                 .Select(monkey => monkey.Inspects)
                                 .Aggregate((long)1, (num, acc) => num * acc));
    }
}