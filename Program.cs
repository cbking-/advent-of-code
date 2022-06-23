using System.Collections.Concurrent;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

var adventType = typeof(AdventOfCode);

var dayToRun = adventType.GetMethod(args[0], BindingFlags.Static | BindingFlags.Public) ?? throw new ArgumentException("Invalid day");

dynamic? task = dayToRun.Invoke(null, null);
await task;

public static class AdventOfCode
{
    #region Helpers
    public static async Task<String> LoadDataAsync(string fileName)
    {
        using var file = new StreamReader(fileName);
        return await file.ReadToEndAsync();
    }

    //https://stackoverflow.com/a/14663233/17400290
    public class IntArrayKeyComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[]? x, int[]? y)
        {
            if (x?.Length != y?.Length)
            {
                return false;
            }
            for (int i = 0; i < x?.Length; i++)
            {
                if (x[i] != y?[i])
                {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(int[] obj)
        {
            int result = 17;
            for (int i = 0; i < obj.Length; i++)
            {
                unchecked
                {
                    result = result * 23 + obj[i];
                }
            }
            return result;
        }
    }
    public class Instruction
    {
        public string LeftOperand { get; set; } = string.Empty;
        public string RightOperand { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public string DestinationWire { get; set; } = string.Empty;
    }
    #endregion

    public static async Task Day1()
    {
        var data = await LoadDataAsync("input1");

        var answer = (data.Where(character => character == '(').Count()) - (data.Where(character => character == ')').Count());

        Console.WriteLine($"Part 1: {answer}");

        var floor = 0;
        answer = 0;

        foreach (var character in data)
        {
            if (floor == -1)
                break;

            answer += 1;

            floor += (character == '(' ? 1 : -1);
        }

        Console.WriteLine($"Part 2: {answer}");
    }

    public static async Task Day2()
    {
        var data = await LoadDataAsync("input2");
        var fixedData = data.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        var answer = fixedData.Select(row => row.Split('x'))
                                  .Sum(dimension =>
                                  {
                                      var length = int.Parse(dimension[0]);
                                      var width = int.Parse(dimension[1]);
                                      var height = int.Parse(dimension[2]);

                                      var sideOne = length * width;
                                      var sideTwo = width * height;
                                      var sideThree = length * height;
                                      var smallSide = (new[] { sideOne, sideTwo, sideThree }).Min();

                                      return (2 * sideOne) + (2 * sideTwo) + (2 * sideThree) + smallSide;
                                  });

        Console.WriteLine($"Part 1: {answer}");

        answer = fixedData.Select(row => row.Split('x'))
                                  .Sum(dimension =>
                                  {
                                      var length = int.Parse(dimension[0]);
                                      var width = int.Parse(dimension[1]);
                                      var height = int.Parse(dimension[2]);
                                      var smallestPerim = (new[] { length, width, height }).OrderBy(num => num).Take(2).Sum() * 2;

                                      var volume = length * width * height;

                                      return smallestPerim + volume;
                                  });

        Console.WriteLine($"Part 2: {answer}");
    }

    public static async Task Day3()
    {
        var data = await LoadDataAsync("input3");

        var houses = new HashSet<int[]>(new IntArrayKeyComparer());
        var x = 0;
        var y = 0;
        houses.Add(new int[] { x, y });

        foreach (var character in data)
        {
            if (character == '^')
                y -= 1;

            if (character == 'v')
                y += 1;

            if (character == '<')
                x -= 1;

            if (character == '>')
                x += 1;

            houses.Add(new int[] { x, y });
        }

        Console.WriteLine($"Part 1: {houses.Count()}");

        houses = new HashSet<int[]>(new IntArrayKeyComparer());
        x = 0;
        y = 0;
        houses.Add(new int[] { x, y });

        foreach (var character in data.Where((character, index) => index % 2 == 0))
        {
            if (character == '^')
                y -= 1;

            if (character == 'v')
                y += 1;

            if (character == '<')
                x -= 1;

            if (character == '>')
                x += 1;

            houses.Add(new int[] { x, y });
        }

        x = 0;
        y = 0;

        foreach (var character in data.Where((character, index) => index % 2 != 0))
        {
            if (character == '^')
                y -= 1;

            if (character == 'v')
                y += 1;

            if (character == '<')
                x -= 1;

            if (character == '>')
                x += 1;

            houses.Add(new int[] { x, y });
        }

        Console.WriteLine($"Part 2: {houses.Count()}");
    }

    public static async Task Day4()
    {
        var data = await LoadDataAsync("input4");

        var answer = 0;

        while (true)
        {
            var md5 = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes($"{data}{answer}")) ?? new byte[] { };

            if (Convert.ToHexString(md5).StartsWith("00000"))
            {
                break;
            }
            answer += 1;
        }

        Console.WriteLine($"Part 1: {answer}");

        answer = 0;
        while (true)
        {
            var md5 = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes($"{data}{answer}")) ?? new byte[] { };

            if (Convert.ToHexString(md5).StartsWith("000000"))
            {
                break;
            }

            answer += 1;
        }

        Console.WriteLine($"Part 2: {answer}");
    }

    public static async Task Day5()
    {
        var data = await LoadDataAsync("input5");

        var lines = data.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        var niceStringRuleOne = "^(.*[aeiou].*){3,}$";
        var niceStringRuleTwo = "([a-z])\\1";
        var niceStringRuleThree = "^(?!.*(ab|cd|pq|xy)).*$";

        var niceStrings = lines.Where(line => Regex.IsMatch(line, niceStringRuleOne)
                                            && Regex.IsMatch(line, niceStringRuleTwo)
                                            && Regex.IsMatch(line, niceStringRuleThree))
                                .Count();

        Console.WriteLine($"Part 1: {niceStrings}");

        niceStringRuleOne = "([a-z][a-z]).*\\1";
        niceStringRuleTwo = "([a-z])[a-z]\\1";

        niceStrings = lines.Where(line => Regex.IsMatch(line, niceStringRuleOne)
                                        && Regex.IsMatch(line, niceStringRuleTwo))
                                .Count();

        Console.WriteLine($"Part 2: {niceStrings}");
    }

    public static async Task Day6()
    {
        var data = await LoadDataAsync("input6");

        var lines = data.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        var instructions = lines.Select(line =>
        {
            return new
            {
                State = Regex.Matches(line, "turn on|turn off|toggle").First().Value,
                Start = Regex.Matches(line, @"\d{0,3},\d{0,3}(?=(\sthrough))").First().Value.Split(',').Select<string, int>(int.Parse).ToArray(),
                End = Regex.Matches(line, @"(?<=(through\s))\d{0,3},\d{0,3}").First().Value.Split(',').Select<string, int>(int.Parse).ToArray()
            };
        });

        var grid = Enumerable.Repeat(false, 1000 * 1000).ToArray();

        foreach (var instruction in instructions)
        {
            var start = new int[2];

            Array.Copy(instruction.Start, start, 2);

            while (start[1] <= instruction.End[1])
            {
                while (start[0] <= instruction.End[0])
                {
                    var location = start[0] + start[1] * 1000;
                    var lightState = grid[location];

                    if (instruction.State == "turn on")
                        lightState = true;

                    if (instruction.State == "turn off")
                        lightState = false;

                    if (instruction.State == "toggle")
                        lightState = !lightState;

                    grid[location] = lightState;

                    start[0] += 1;
                }

                start[0] = instruction.Start[0];
                start[1] += 1;
            }
        }

        Console.WriteLine($"Part 1: {grid.Count(light => light)}");

        var grid2 = Enumerable.Repeat(0, 1000 * 1000).ToArray();

        foreach (var instruction in instructions)
        {
            var start = new int[2];

            Array.Copy(instruction.Start, start, 2);

            while (start[1] <= instruction.End[1])
            {
                while (start[0] <= instruction.End[0])
                {
                    var location = start[0] + start[1] * 1000;
                    var lightState = grid2[location];

                    if (instruction.State == "turn on")
                        lightState += 1;

                    if (instruction.State == "turn off")
                        lightState = lightState - 1 >= 0 ? lightState - 1 : 0;

                    if (instruction.State == "toggle")
                        lightState += 2;

                    grid2[location] = lightState;

                    start[0] += 1;
                }

                start[0] = instruction.Start[0];
                start[1] += 1;
            }
        }

        Console.WriteLine($"Part 2: {grid2.Sum(light => light)}");
    }

    public static async Task Day7()
    {
        //There's probably signficantly more efficient ways to do this one
        // but was fun to figure out 
        // 10/10
        #region Setup
        var data = await LoadDataAsync("input7");

        var lines = data.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        var instructions = lines.Select(line =>
        {
            var sources = line.Split(" -> ")[0];
            var destination = line.Split(" -> ")[1];

            var instruction = new Instruction();

            if (sources.StartsWith("NOT"))
            {
                instruction.Operation = sources.Split(" ")[0];
                instruction.RightOperand = sources.Split(" ")[1];
            }
            else if (sources.Split(" ").Length == 1)
            {
                instruction.RightOperand = sources;
            }
            else
            {
                instruction.LeftOperand = sources.Split(" ")[0];
                instruction.Operation = sources.Split(" ")[1];
                instruction.RightOperand = sources.Split(" ")[2];
            }

            instruction.DestinationWire = destination;

            return instruction;
        });

        ConcurrentDictionary<string, int> map = new(instructions.Where(instruction => int.TryParse(instruction.RightOperand, out var temp) && instruction.LeftOperand == string.Empty)
                            .ToDictionary(item => item.DestinationWire, item => int.Parse(item.RightOperand)));
        #endregion

        #region Part1
        while (!map.ContainsKey("a"))
        {
            var instructionsToSolveNext = instructions.Where(instruction => (map.ContainsKey(instruction.LeftOperand) && map.ContainsKey(instruction.RightOperand)) //two wires
                                                || (map.ContainsKey(instruction.RightOperand) && string.IsNullOrWhiteSpace(instruction.LeftOperand)) //NOT, wire -> wire
                                                || (instruction.LeftOperand == "1" && map.ContainsKey(instruction.RightOperand)) // 1 AND wire
                                                || (map.ContainsKey(instruction.LeftOperand) && int.TryParse(instruction.RightOperand, out var temp))); //wire SHIFT num

            instructionsToSolveNext.AsParallel().ForAll(instruction =>
            {
                map.TryGetValue(instruction.LeftOperand, out var valueOne);
                map.TryGetValue(instruction.RightOperand, out var valueTwo);

                var finalValue = 0;

                if (string.IsNullOrWhiteSpace(instruction.Operation))
                {
                    finalValue = valueTwo;
                }
                else if (instruction.Operation == "NOT")
                {
                    finalValue = ~valueTwo;
                }
                else if (instruction.Operation == "AND")
                {
                    if (instruction.LeftOperand == "1")
                    {
                        finalValue = 1 & valueTwo;
                    }
                    else
                    {
                        finalValue = valueOne & valueTwo;
                    }
                }
                else if (instruction.Operation == "OR")
                {
                    finalValue = valueOne | valueTwo;
                }
                else if (instruction.Operation == "RSHIFT")
                {
                    finalValue = valueOne >> int.Parse(instruction.RightOperand);
                }
                else if (instruction.Operation == "LSHIFT")
                {
                    finalValue = valueOne << int.Parse(instruction.RightOperand);
                }

                map.TryAdd(instruction.DestinationWire, finalValue);
            });
        }

        Console.WriteLine($"Part 1: {map["a"]}");
        #endregion

        #region Par2
        var newSeed = map["a"];

        map = new(instructions.Where(instruction => int.TryParse(instruction.RightOperand, out var temp) && instruction.LeftOperand == string.Empty)
                            .ToDictionary(item => item.DestinationWire, item => int.Parse(item.RightOperand)));

        map["b"] = newSeed;

        while (!map.ContainsKey("a"))
        {
            var instructionsToSolveNext = instructions.Where(instruction => (map.ContainsKey(instruction.LeftOperand) && map.ContainsKey(instruction.RightOperand)) //two wires
                                                || (map.ContainsKey(instruction.RightOperand) && string.IsNullOrWhiteSpace(instruction.LeftOperand)) //NOT, wire -> wire
                                                || (instruction.LeftOperand == "1" && map.ContainsKey(instruction.RightOperand)) // 1 AND wire
                                                || (map.ContainsKey(instruction.LeftOperand) && int.TryParse(instruction.RightOperand, out var temp))); //wire SHIFT num

            instructionsToSolveNext.AsParallel().ForAll(instruction =>
            {
                map.TryGetValue(instruction.LeftOperand, out var valueOne);
                map.TryGetValue(instruction.RightOperand, out var valueTwo);

                var finalValue = 0;

                if (string.IsNullOrWhiteSpace(instruction.Operation))
                {
                    finalValue = valueTwo;
                }
                else if (instruction.Operation == "NOT")
                {
                    finalValue = ~valueTwo;
                }
                else if (instruction.Operation == "AND")
                {
                    if (instruction.LeftOperand == "1")
                    {
                        finalValue = 1 & valueTwo;
                    }
                    else
                    {
                        finalValue = valueOne & valueTwo;
                    }
                }
                else if (instruction.Operation == "OR")
                {
                    finalValue = valueOne | valueTwo;
                }
                else if (instruction.Operation == "RSHIFT")
                {
                    finalValue = valueOne >> int.Parse(instruction.RightOperand);
                }
                else if (instruction.Operation == "LSHIFT")
                {
                    finalValue = valueOne << int.Parse(instruction.RightOperand);
                }

                map.TryAdd(instruction.DestinationWire, finalValue);
            });
        }

        Console.WriteLine($"Part 2: {map["a"]}");
        #endregion
    }
}
