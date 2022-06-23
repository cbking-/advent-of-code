using System.Collections.Concurrent;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

var adventType = typeof(AdventOfCode);

var dataToLoad = adventType.GetMethod("LoadDataAsync", BindingFlags.Static | BindingFlags.Public) ?? throw new ArgumentException("Method not found");
dynamic? loadTask = dataToLoad.Invoke(null, new object[] { $"inputs\\{args[0]}.txt" });
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

    public class TSPVertex
    {
        public string City { get; set; } = string.Empty;

        public bool Visited { get; set; } = false;

        public Dictionary<string, int> Neighbors { get; set; } = new Dictionary<string, int>();
    }

    //https://stackoverflow.com/a/15150493/17400290
    public static IEnumerable<IEnumerable<T>> Permute<T>(this IEnumerable<T> input)
    {
        if (input == null)
        {
            yield break;
        }

        var list = input.ToList();

        if (!list.Any())
        {
            yield return Enumerable.Empty<T>();
        }
        else
        {
            var startingElementIndex = 0;

            foreach (var startingElement in list)
            {
                var index = startingElementIndex;
                var remainingItems = list.Where((e, i) => i != index);

                foreach (var permutationOfRemainder in remainingItems.Permute())
                {
                    yield return permutationOfRemainder.Prepend(startingElement);
                }

                startingElementIndex++;
            }
        }
    }

    //based on http://www.herlitz.nu/2011/12/01/getting-the-previous-and-next-record-from-list-using-linq/
    ///<summary>
    /// Gets the next value or, if at the end of the list, gets the first value.
    ///</summary>
    public static T? GetNext<T>(IEnumerable<T> list, T current)
    {
        try
        {
            return list.SkipWhile(x => x != null && !x.Equals(current)).Skip(1).FirstOrDefault() ?? list.First();
        }
        catch
        {
            return default(T);
        }
    }

    //based on http://www.herlitz.nu/2011/12/01/getting-the-previous-and-next-record-from-list-using-linq/
    ///<summary>
    /// Gets the previous value or, if at the beginning of the list, gets the last value.
    ///</summary>
    public static T? GetPrevious<T>(IEnumerable<T> list, T current)
    {
        try
        {
            return list.TakeWhile(x => x != null && !x.Equals(current)).LastOrDefault() ?? list.Last();
        }
        catch
        {
            return default(T);
        }
    }

    public class Ingredient
    {
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; } = 0;
        public int Durability { get; set; } = 0;
        public int Flavor { get; set; } = 0;
        public int Texture { get; set; } = 0;
        public int Calories { get; set; } = 0;
    }

    //https://rosettacode.org/wiki/Combinations_with_repetitions#C.23
    public static List<List<T>> GenerateCombinations<T>(List<T> combinationList, int selectionSize)
    {
        var combinations = new List<List<T>>();

        if (selectionSize == 0)
        {
            var emptyCombination = new List<T>();
            combinations.Add(emptyCombination);

            return combinations;
        }

        if (combinationList.Count == 0)
        {
            return combinations;
        }

        T head = combinationList[0];
        var copiedCombinationList = new List<T>(combinationList);

        List<List<T>> subcombinations = GenerateCombinations(copiedCombinationList, selectionSize - 1);

        foreach (var subcombination in subcombinations)
        {
            subcombination.Insert(0, head);
            combinations.Add(subcombination);
        }

        combinationList.RemoveAt(0);
        combinations.AddRange(GenerateCombinations(combinationList, selectionSize));

        return combinations;
    }

    public class Aunt
    {
        public int Children { get; set; } = 0;

        public int Cats { get; set; } = 0;

        public int Samoyeds {get; set;} = 0;

        public int Pomeranians {get; set; } = 0;

        public int Akitas {get; set;} = 0;

        public int Vizslas {get; set; } = 0;

        public int Goldfish {get; set; } = 0;

        public int Trees {get; set;} = 0;

        public int Cars {get; set;} = 0;

        public int Perfumes {get; set; } = 0;
    }
    #endregion

    public static void Day1(string[] data)
    {
        var answer = (data[0].Where(character => character == '(').Count()) - (data[0].Where(character => character == ')').Count());

        Console.WriteLine($"Part 1: {answer}");

        var floor = 0;
        answer = 0;

        foreach (var character in data[0])
        {
            if (floor == -1)
                break;

            answer += 1;

            floor += (character == '(' ? 1 : -1);
        }

        Console.WriteLine($"Part 2: {answer}");
    }

    public static void Day2(string[] data)
    {
        var answer = data.Select(row => row.Split('x'))
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

        answer = data.Select(row => row.Split('x'))
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

    public static void Day3(string[] data)
    {
        var houses = new HashSet<int[]>(new IntArrayKeyComparer());
        var x = 0;
        var y = 0;
        houses.Add(new int[] { x, y });

        foreach (var character in data[0])
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

        foreach (var character in data[0].Where((character, index) => index % 2 == 0))
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

        foreach (var character in data[0].Where((character, index) => index % 2 != 0))
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

    public static void Day4(string[] data)
    {
        var answer = 0;

        while (true)
        {
            var md5 = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes($"{data[0]}{answer}")) ?? new byte[] { };

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
            var md5 = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes($"{data[0]}{answer}")) ?? new byte[] { };

            if (Convert.ToHexString(md5).StartsWith("000000"))
            {
                break;
            }

            answer += 1;
        }

        Console.WriteLine($"Part 2: {answer}");
    }

    public static void Day5(string[] data)
    {
        var niceStringRuleOne = "^(.*[aeiou].*){3,}$";
        var niceStringRuleTwo = "([a-z])\\1";
        var niceStringRuleThree = "^(?!.*(ab|cd|pq|xy)).*$";

        var niceStrings = data.Where(line => Regex.IsMatch(line, niceStringRuleOne)
                                            && Regex.IsMatch(line, niceStringRuleTwo)
                                            && Regex.IsMatch(line, niceStringRuleThree))
                                .Count();

        Console.WriteLine($"Part 1: {niceStrings}");

        niceStringRuleOne = "([a-z][a-z]).*\\1";
        niceStringRuleTwo = "([a-z])[a-z]\\1";

        niceStrings = data.Where(line => Regex.IsMatch(line, niceStringRuleOne)
                                        && Regex.IsMatch(line, niceStringRuleTwo))
                                .Count();

        Console.WriteLine($"Part 2: {niceStrings}");
    }

    public static void Day6(string[] data)
    {
        var instructions = data.Select(line =>
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

    public static void Day7(string[] data)
    {
        //There's probably signficantly more efficient ways to do this one
        // but was fun to figure out 
        // 10/10
        #region Setup

        var instructions = data.Select(line =>
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

    public static void Day8(string[] data)
    {
        var characterSum = data.Sum(line => line.Length);

        var memorySum = data.Sum(line =>
        {
            line = line.Substring(1, line.Length - 2);
            line = Regex.Replace(line, @"\\\\", @"A");
            line = Regex.Replace(line, @"\\""", @"A");
            line = Regex.Replace(line, @"\\x[0-9a-fA-F]{2}", @"A");
            return line.Length;
        });

        Console.WriteLine($"Part 1: {characterSum - memorySum}");

        var encodedSum = data.Sum(line =>
        {
            //some lines end with \\" which will mess up replacment later
            line = line.Substring(1, line.Length - 2);
            line = Regex.Replace(line, @"\\", @"\\");
            line = Regex.Replace(line, @"\\""", @"\\""");

            return line.Length + 6; //4 being the encode quotes and 2 the surrounding quotes
        });

        Console.WriteLine($"Part 2: {encodedSum - characterSum}");
    }

    public static void Day9(string[] data)
    {
        var vertices = new List<TSPVertex>();

        foreach (var line in data)
        {
            var pattern = @"(\w+) to (\w+) = (\d+)";
            var matches = Regex.Matches(line, pattern);

            var Distance = int.Parse(matches.First().Groups[3].Value);
            var City = matches.First().Groups[1].Value;
            var Neighbor = matches.First().Groups[2].Value;

            var vertex = vertices.Where(route => route.City == City).SingleOrDefault() ?? new TSPVertex { City = City, Visited = false };
            var neightborVertex = vertices.Where(route => route.City == Neighbor).SingleOrDefault() ?? new TSPVertex { City = Neighbor, Visited = false };

            if (!vertices.Any(route => route.City == vertex.City))
            {
                vertex.Neighbors.Add(Neighbor, Distance);
                vertices.Add(vertex);
            }
            else
            {
                vertices.Where(route => route.City == City).Select(route => { route.Neighbors.Add(Neighbor, Distance); return route; }).ToList();
            }

            //Also add neighbor vertex
            if (!vertices.Any(route => route.City == neightborVertex.City))
            {
                neightborVertex.Neighbors.Add(City, Distance);
                vertices.Add(neightborVertex);
            }
            else
            {
                vertices.Where(route => route.City == Neighbor).Select(route => { route.Neighbors.Add(City, Distance); return route; }).ToList();
            }

        }

        //Traveling Sales Person Problem
        //nearest neightbor algorithm

        // 1. Initialize all vertices as unvisited.
        // 2. Select an arbitrary vertex, set it as the current vertex u. Mark u as visited.
        // 3. Find out the shortest edge connecting the current vertex u and an unvisited vertex v.
        // 4. Set v as the current vertex u. Mark v as visited.
        // 5. If all the vertices in the domain are visited, then terminate. Else, go to step 3.

        var distance = int.MaxValue;

        //using each vertex as a staring point
        foreach (var vertex in vertices)
        {
            var currentVertex = vertex;
            var iterationDistance = 0;

            //loop until ever vertext has been visisted      
            while (vertices.Any(vertex => vertex.Visited == false))
            {
                //null check so c# stops complaining
                if (currentVertex is null)
                {
                    break;
                }

                currentVertex.Visited = true;
                var nearestNeighbor = currentVertex.Neighbors.Where(neighbor => vertices.Any(vertex => vertex.City == neighbor.Key && !vertex.Visited))
                                                             .OrderBy(kvp => kvp.Value).FirstOrDefault();
                iterationDistance += nearestNeighbor.Value;
                currentVertex = vertices.Where(vertex => vertex.City == nearestNeighbor.Key && !vertex.Visited).SingleOrDefault();
            }

            distance = Math.Min(distance, iterationDistance);

            //reset visitation
            vertices.Select(vertex => { vertex.Visited = false; return vertex; }).ToList();
        }

        Console.WriteLine($"Part 1: {distance}");

        distance = 0;

        foreach (var vertex in vertices)
        {
            var currentVertex = vertex;
            var iterationDistance = 0;

            while (vertices.Any(vertex => vertex.Visited == false))
            {
                if (currentVertex is null)
                {
                    break;
                }

                currentVertex.Visited = true;
                var nearestNeighbor = currentVertex.Neighbors.Where(neighbor => vertices.Any(vertex => vertex.City == neighbor.Key && !vertex.Visited))
                                                             .OrderByDescending(kvp => kvp.Value).FirstOrDefault();
                iterationDistance += nearestNeighbor.Value;
                currentVertex = vertices.Where(vertex => vertex.City == nearestNeighbor.Key && !vertex.Visited).SingleOrDefault();
            }

            distance = Math.Max(distance, iterationDistance);

            vertices.Select(vertex => { vertex.Visited = false; return vertex; }).ToList();
        }

        Console.WriteLine($"Part 2: {distance}");
    }

    public static void Day10(string[] data)
    {
        var iteration = data[0];

        var pattern = @"((\d)\2{0,})+";

        for (var step = 1; step <= 40; step++)
        {
            var captures = Regex.Match(iteration, pattern).Groups[1].Captures;

            iteration = string.Concat(captures.Select(capture => capture.Length.ToString() + capture.Value.Last()));
        }

        Console.WriteLine($"Part 1: {iteration.Length}");

        iteration = data[0];

        for (var step = 1; step <= 50; step++)
        {
            var captures = Regex.Match(iteration, pattern).Groups[1].Captures;

            iteration = string.Concat(captures.Select(capture => capture.Length.ToString() + capture.Value.Last()));
        }

        Console.WriteLine($"Part 1: {iteration.Length}");
    }

    public static void Day11(string[] data)
    {
        var straightPattern = "(abc|bcd|cde|def|efg|fgh|pqr|qrs|rst|stu|tuv|uvw|vwx|wxy|xyz)";
        var invalidCharactersPattern = "[iol]";

        //This rule isn't super clear to me from the instructions on the site
        // Is "aabaa" supposed to be valid?
        // My best guess is aabaa is valid. "Non-overlapping" means something like "aaa" is invalid
        // "Two different" means positionally different (at least one character between pairs) not that the pairs are
        // different letters.
        var pairPairsPattern = @"(.)\1.*(.)\2";

        var isValid = false;
        var builder = new StringBuilder(data[0]);

        while (!isValid)
        {
            for (var index = builder.Length - 1; index > 0; index--)
            {
                if (builder[index] == 'z')
                {
                    builder[index] = 'a';
                }
                else
                {
                    builder[index] = (Char)(Convert.ToUInt16(builder[index]) + 1);
                    break;
                }
            }

            if (Regex.IsMatch(builder.ToString(), straightPattern)
                && !Regex.IsMatch(builder.ToString(), invalidCharactersPattern)
                && Regex.IsMatch(builder.ToString(), pairPairsPattern))
            {
                isValid = true;
            }
        }

        Console.WriteLine($"Part 1: {builder.ToString()}");

        isValid = false;

        while (!isValid)
        {
            for (var index = builder.Length - 1; index > 0; index--)
            {
                if (builder[index] == 'z')
                {
                    builder[index] = 'a';
                }
                else
                {
                    builder[index] = (Char)(Convert.ToUInt16(builder[index]) + 1);
                    break;
                }
            }

            if (Regex.IsMatch(builder.ToString(), straightPattern)
                && !Regex.IsMatch(builder.ToString(), invalidCharactersPattern)
                && Regex.IsMatch(builder.ToString(), pairPairsPattern))
            {
                isValid = true;
            }
        }

        Console.WriteLine($"Part 2: {builder.ToString()}");
    }

    public static void Day12(string[] data)
    {
        var sum = Regex.Matches(data[0], @"-?\d+").Sum(match => int.Parse(match.Value));
        Console.WriteLine($"Part 1: {sum}");

        //Newtonsoft's Json.NET is much easier to work with 
        // in this case than System.Text.Json
        var json = JObject.Parse(data[0]);

        json.Descendants()
            .OfType<JObject>()
            .Where(obj => obj.Properties().Values().Any(val => val.Type == JTokenType.String && val.Value<string>() == "red"))
            .ToList()
            .ForEach(obj =>
            {
                try
                {
                    obj.Remove();
                }
                catch
                {
                    //Can't remove the value from a JProperty so remove the JProperty
                    obj.Parent?.Remove();
                }
            });

        var serialized = JsonConvert.SerializeObject(json);

        sum = Regex.Matches(serialized, @"-?\d+").Sum(match => int.Parse(match.Value));
        Console.WriteLine($"Part 2: {sum}");
    }

    public static void Day13(string[] data)
    {
        //At first, I thought this would be pretty similar to how I
        // solved Day 9. However, I found that the nearest neighbor algorithm
        // doesn't work for this problem as well as it did for the other.
        // I think it has to do with how each vertex has two neighbors (rather than a boolean 'visited')
        // and creates a circuit rather than a straight path.
        // I'm not smart enough to figure out how to get nearest neighbor to work here.

        //will maintain original values
        var vertices = new List<TSPVertex>();

        foreach (var line in data)
        {
            var pattern = @"(\w+) would (\w+) (\d+) happiness units by sitting next to (\w+)";
            var matches = Regex.Matches(line, pattern);

            var match = matches.First();

            var sign = match.Groups[2].Value == "gain" ? "+" : "-";

            var Distance = int.Parse(sign + match.Groups[3].Value);
            var City = match.Groups[1].Value;
            var Neighbor = match.Groups[4].Value;

            var vertex = vertices.Where(route => route.City == City).SingleOrDefault() ?? new TSPVertex { City = City, Visited = false };

            if (!vertices.Any(route => route.City == vertex.City))
            {
                vertex.Neighbors.Add(Neighbor, Distance);
                vertices.Add(vertex);
            }
            else
            {
                vertices.Where(route => route.City == City).Select(route => { route.Neighbors.Add(Neighbor, Distance); return route; }).ToList();
            }
        }

        //used to save calculated values
        var updatedVertices = JsonConvert.DeserializeObject<List<TSPVertex>>(JsonConvert.SerializeObject(vertices)) ?? new List<TSPVertex>();

        //sum vertex and neighbors' happiness values to get delta happiness
        foreach (var vertex in vertices)
        {
            foreach (var neighbor in vertex.Neighbors)
            {
                var neighborVertexHappiness = vertices.Where(vert => vert.City == neighbor.Key)
                                                      .Single()
                                                      .Neighbors
                                                      .Where(kvp => kvp.Key == vertex.City)
                                                      .Single()
                                                      .Value;

                updatedVertices.Where(vert => vert.City == vertex.City).Single().Neighbors[neighbor.Key] += neighborVertexHappiness;
            }
        }

        var happiness = 0;

        //nearest neighbor algorithm doesn't work for this one
        foreach (var permutation in updatedVertices.Permute())
        {
            var iterationHappiness = 0;

            foreach (var vertex in permutation)
            {
                iterationHappiness += vertex.Neighbors.Where(kvp => kvp.Key == GetNext(permutation, vertex)?.City).Single().Value;
            }

            happiness = Math.Max(happiness, iterationHappiness);
        }

        Console.WriteLine($"Part 1: {happiness}");

        updatedVertices.Add(new TSPVertex
        {
            City = "Corbin",
            Visited = false
        });

        foreach (var vertex in updatedVertices)
        {
            if (vertex.City != "Corbin")
            {
                vertex.Neighbors.Add("Corbin", 0);
                updatedVertices.Where(vert => vert.City == "Corbin").Single().Neighbors.Add(vertex.City, 0);
            }
        }

        happiness = 0;

        foreach (var permutation in updatedVertices.Permute())
        {
            var iterationHappiness = 0;

            foreach (var vertex in permutation)
            {
                iterationHappiness += vertex.Neighbors.Where(kvp => kvp.Key == GetNext(permutation, vertex)?.City).Single().Value;
            }

            happiness = Math.Max(happiness, iterationHappiness);
        }

        Console.WriteLine($"Part 2: {happiness}");
    }

    public static void Day14(string[] data)
    {
        var winner = 0;
        var raceDuration = 2503;

        foreach (var line in data)
        {
            var pattern = @"(\w+) can fly (\d+) km/s for (\d+) seconds, but then must rest for (\d+) seconds.";
            var match = Regex.Matches(line, pattern).First();

            var reindeer = match.Groups[1].Value;
            var speed = int.Parse(match.Groups[2].Value);
            var speedDuration = int.Parse(match.Groups[3].Value);
            var restDuration = int.Parse(match.Groups[4].Value);

            var distance = speed * speedDuration;
            var totalDuration = speedDuration + restDuration;

            var totalDistance = distance * (raceDuration / totalDuration);
            var speedTimeLeft = raceDuration % totalDuration;

            if (speedTimeLeft >= speedDuration)
            {
                totalDistance += distance;
            }
            else
            {
                totalDistance += speed * speedTimeLeft;
            }

            winner = Math.Max(winner, totalDistance);
        }

        Console.WriteLine($"Part 1: {winner}");

        var race = new Dictionary<string, int[]>();
        var points = new Dictionary<string, int>();

        foreach (var line in data)
        {
            var pattern = @"(\w+) can fly (\d+) km/s for (\d+) seconds, but then must rest for (\d+) seconds.";
            var match = Regex.Matches(line, pattern).First();

            var reindeer = match.Groups[1].Value;
            var speed = int.Parse(match.Groups[2].Value);
            var speedDuration = int.Parse(match.Groups[3].Value);
            var restDuration = int.Parse(match.Groups[4].Value);

            var distance = speed * speedDuration;
            var totalDuration = speedDuration + restDuration;

            race.Add(reindeer, Enumerable.Repeat(0, raceDuration).ToArray());
            points.Add(reindeer, 0);
            var accumulator = 0;

            foreach (var seconds in Enumerable.Range(1, raceDuration))
            {
                var totalDistance = distance * (seconds / totalDuration);
                var speedTimeLeft = seconds % totalDuration;

                if (speedTimeLeft > speedDuration)
                {
                    race[reindeer][seconds - 1] = accumulator;
                }
                else
                {
                    race[reindeer][seconds - 1] = (speed * speedTimeLeft) + totalDistance;
                    accumulator = (speed * speedTimeLeft) + totalDistance;
                }
            }
        }


        foreach (var second in Enumerable.Range(1, raceDuration))
        {
            var momentWinner = race.Max(reindeer => reindeer.Value[second - 1]);
            var scorer = race.First(reindeer => reindeer.Value[second - 1] == momentWinner).Key;
            points[scorer] += 1;
        }

        Console.WriteLine($"Part 2: {points.Max(kvp => kvp.Value)}");
    }

    public static void Day15(string[] data)
    {
        var ingredients = new List<Ingredient>();

        foreach (var line in data)
        {
            //Sugar: capacity 3, durability 0, flavor 0, texture -3, calories 2
            var pattern = @"(\w+): capacity (-?\d+), durability (-?\d+), flavor (-?\d+), texture (-?\d+), calories (-?\d+)";
            var match = Regex.Matches(line, pattern).First();

            ingredients.Add(new Ingredient
            {
                Name = match.Groups[1].Value,
                Capacity = int.Parse(match.Groups[2].Value),
                Durability = int.Parse(match.Groups[3].Value),
                Flavor = int.Parse(match.Groups[4].Value),
                Texture = int.Parse(match.Groups[5].Value),
                Calories = int.Parse(match.Groups[6].Value)
            });
        }

        var combinations = GenerateCombinations(ingredients, 100);
        var bestScore = 0;
        var bestScoreWithCalories = 0;

        foreach (var combo in combinations)
        {
            var groups = combo.GroupBy(ingredient => ingredient.Name);
            var capacity = Math.Clamp(groups.Sum(group => group.Sum(ingredient => ingredient.Capacity)), 0, int.MaxValue);
            var durability = Math.Clamp(groups.Sum(group => group.Sum(ingredient => ingredient.Durability)), 0, int.MaxValue);
            var flavor = Math.Clamp(groups.Sum(group => group.Sum(ingredient => ingredient.Flavor)), 0, int.MaxValue);
            var texture = Math.Clamp(groups.Sum(group => group.Sum(ingredient => ingredient.Texture)), 0, int.MaxValue);

            var calories = groups.Sum(group => group.Sum(ingredient => ingredient.Calories));

            if (calories == 500)
            {
                bestScoreWithCalories = Math.Max(bestScoreWithCalories, capacity * durability * flavor * texture);
            }

            bestScore = Math.Max(bestScore, capacity * durability * flavor * texture);
        }

        Console.WriteLine($"Part 1: {bestScore}");
        Console.WriteLine($"Part 2: {bestScoreWithCalories}");
    }

    public static void Day16(string[] data)
    {
        // children: 3
        // cats: 7
        // samoyeds: 2
        // pomeranians: 3
        // akitas: 0
        // vizslas: 0
        // goldfish: 5
        // trees: 3
        // cars: 2
        // perfumes: 1
    }
}
