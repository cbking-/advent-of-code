using System.Collections.Concurrent;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Combinatorics.Collections;
using System.Diagnostics;
using static Core.Helpers;

var data = await LoadDataAsync(args[0]);

var adventType = typeof(AdventOfCode);
var dayToRun = adventType.GetMethod(args[0], BindingFlags.Static | BindingFlags.Public) ?? throw new ArgumentException("Invalid day");
dayToRun.Invoke(null, new object[] { data });

public static class AdventOfCode
{
    #region Objects    

    public class Instruction
    {
        public string LeftOperand { get; set; } = string.Empty;
        public string RightOperand { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public string DestinationWire { get; set; } = string.Empty;
    }

    public class TSPVertex : IComparable
    {
        public string City { get; set; } = string.Empty;

        public bool Visited { get; set; } = false;

        public Dictionary<string, int> Neighbors { get; set; } = new Dictionary<string, int>();

        public int CompareTo(object? obj)
        {
            if (obj == null) return 1;

            TSPVertex? otherVertex = obj as TSPVertex;
            if (otherVertex != null)
                return this.City.CompareTo(otherVertex.City);
            else
                throw new ArgumentException("Object is not a vertex");
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

    public class Aunt
    {
        [JsonIgnore]
        public int Number { get; set; } = 0;

        [JsonProperty("children")]
        public int? Children { get; set; }

        [JsonProperty("cats")]
        public int? Cats { get; set; }

        [JsonProperty("samoyeds")]
        public int? Samoyeds { get; set; }

        [JsonProperty("pomeranians")]
        public int? Pomeranians { get; set; }

        [JsonProperty("akitas")]
        public int? Akitas { get; set; }

        [JsonProperty("vizslas")]
        public int? Vizslas { get; set; }

        [JsonProperty("goldfish")]
        public int? Goldfish { get; set; }

        [JsonProperty("trees")]
        public int? Trees { get; set; }

        [JsonProperty("cars")]
        public int? Cars { get; set; }

        [JsonProperty("perfumes")]
        public int? Perfumes { get; set; }
    }

    [DebuggerDisplay("{Name}")]
    public class RPGItem : IComparable
    {
        public string Name { get; set; } = string.Empty;

        public int Cost { get; set; } = 0;

        public int Damage { get; set; } = 0;

        public int Armor { get; set; } = 0;

        public string Type { get; set; } = "Weapon";

        public int CompareTo(object? obj)
        {
            if (obj == null) return 1;

            RPGItem? otherItem = obj as RPGItem;
            if (otherItem != null)
                return this.Name.CompareTo(otherItem.Name);
            else
                throw new ArgumentException("Object is not an RPG item");
        }
    }

    public class Player
    {
        public int Health { get; set; } = 0;
        public int Damage { get; set; } = 0;
        public int Mana { get; set; } = 0;
        public int ManaSpent { get; set; } = 0;
        public int Armor {get; set;} = 0;
        public List<Spell> Spells { get; set; } = new List<Spell>();

        public Player Copy()
        {
            return JsonConvert.DeserializeObject<Player>(JsonConvert.SerializeObject(this)) ?? new Player();
        }
    }

    [DebuggerDisplay("{Name} {Effect.Active}")]
    public class Spell : IComparable
    {
        public string Name { get; set; } = string.Empty;

        public int Cost { get; set; } = 0;

        public Effect Effect { get; set; } = new Effect();

        public int CompareTo(object? obj)
        {
            if (obj == null) return 1;

            Spell? otherSpell = obj as Spell;
            if (otherSpell != null)
                return this.Name.CompareTo(otherSpell.Name);
            else
                throw new ArgumentException("Object is not a Spell");
        }
    }

    public class Effect : ICloneable
    {
        public int Damage { get; set; } = 0;
        public int Heal { get; set; } = 0;
        public int Time { get; set; } = 0;
        public int Armor { get; set; } = 0;
        public int Mana { get; set; } = 0;
        public int Tick { get; set; } = 0;
        public bool Active { get; set; } = false;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
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
        var width = 1000;
        var instructions = data.Select(line =>
        {
            return new
            {
                State = Regex.Matches(line, "turn on|turn off|toggle").First().Value,
                Start = Regex.Matches(line, @"\d{0,3},\d{0,3}(?=(\sthrough))").First().Value.Split(',').Select<string, int>(int.Parse).ToArray(),
                End = Regex.Matches(line, @"(?<=(through\s))\d{0,3},\d{0,3}").First().Value.Split(',').Select<string, int>(int.Parse).ToArray()
            };
        });

        var grid = Enumerable.Repeat(false, width * width).ToArray();

        foreach (var instruction in instructions)
        {
            var start = new int[2];

            Array.Copy(instruction.Start, start, 2);

            while (start[1] <= instruction.End[1])
            {
                while (start[0] <= instruction.End[0])
                {
                    var location = start[0] + start[1] * width;
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

        var grid2 = Enumerable.Repeat(0, width * width).ToArray();

        foreach (var instruction in instructions)
        {
            var start = new int[2];

            Array.Copy(instruction.Start, start, 2);

            while (start[1] <= instruction.End[1])
            {
                while (start[0] <= instruction.End[0])
                {
                    var location = start[0] + start[1] * width;
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
        foreach (var permutation in new Permutations<TSPVertex>(updatedVertices))
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

        foreach (var permutation in new Permutations<TSPVertex>(updatedVertices))
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

        var combinations = new Combinations<Ingredient>(ingredients, 100, GenerateOption.WithRepetition);
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
        var auntToFind = new Aunt
        {
            Children = 3,
            Cats = 7,
            Samoyeds = 2,
            Pomeranians = 3,
            Akitas = 0,
            Vizslas = 0,
            Goldfish = 5,
            Trees = 3,
            Cars = 2,
            Perfumes = 1
        };

        var aunts = new List<Aunt>();
        foreach (var line in data.Select((value, index) => new { index, value }))
        {
            //converting line to JSON so it's easily parsed to object
            var sue = Regex.Replace(Regex.Replace(line.value, @"Sue \d+:", ""), @"(?<=\s)(\w+)(?=:)", @"""$1""");

            sue = $"{{ {sue} }}";

            var aunt = JsonConvert.DeserializeObject<Aunt>(sue) ?? new Aunt();
            aunt.Number = line.index + 1;
            aunts.Add(aunt);
        }

        //https://stackoverflow.com/questions/31114892/using-linq-to-find-best-match-across-multiple-properties
        var grouping = aunts.GroupBy(aunt =>
            (aunt.Akitas == auntToFind.Akitas ? 1 : 0) +
            (aunt.Cars == auntToFind.Cars ? 1 : 0) +
            (aunt.Cats == auntToFind.Cats ? 1 : 0) +
            (aunt.Children == auntToFind.Children ? 1 : 0) +
            (aunt.Goldfish == auntToFind.Goldfish ? 1 : 0) +
            (aunt.Perfumes == auntToFind.Perfumes ? 1 : 0) +
            (aunt.Pomeranians == auntToFind.Pomeranians ? 1 : 0) +
            (aunt.Samoyeds == auntToFind.Samoyeds ? 1 : 0) +
            (aunt.Trees == auntToFind.Trees ? 1 : 0) +
            (aunt.Vizslas == auntToFind.Vizslas ? 1 : 0)
        );

        var maxCount = grouping.Max(x => x.Key);
        var resultSet = grouping.FirstOrDefault(x => x.Key == maxCount)?.Select(g => g).Single();
        Console.WriteLine($"Part 1: {resultSet?.Number}");

        grouping = aunts.GroupBy(aunt =>
            (aunt.Akitas == auntToFind.Akitas ? 1 : 0) +
            (aunt.Cars == auntToFind.Cars ? 1 : 0) +
            (aunt.Cats > auntToFind.Cats ? 1 : 0) +
            (aunt.Children == auntToFind.Children ? 1 : 0) +
            (aunt.Goldfish < auntToFind.Goldfish ? 1 : 0) +
            (aunt.Perfumes == auntToFind.Perfumes ? 1 : 0) +
            (aunt.Pomeranians < auntToFind.Pomeranians ? 1 : 0) +
            (aunt.Samoyeds == auntToFind.Samoyeds ? 1 : 0) +
            (aunt.Trees > auntToFind.Trees ? 1 : 0) +
            (aunt.Vizslas == auntToFind.Vizslas ? 1 : 0)
        );

        maxCount = grouping.Max(x => x.Key);
        resultSet = grouping.FirstOrDefault(x => x.Key == maxCount)?.Select(g => g).Single();
        Console.WriteLine($"Part 1: {resultSet?.Number}");
    }

    public static void Day17(string[] data)
    {
        var totalWays = 0;
        var minTotalWays = new ConcurrentDictionary<int, int>();

        for (var i = 1; i <= data.Count(); i++)
        {
            foreach (var combination in new Combinations<int>(data.ToList().ConvertAll(int.Parse), i, GenerateOption.WithoutRepetition))
            {
                if (combination.Sum() == 150)
                {
                    minTotalWays.AddOrUpdate(i, 1, (key, oldValue) => oldValue + 1);
                    totalWays += 1;
                }
            }
        }

        Console.WriteLine($"Part 1: {totalWays}");
        Console.WriteLine($"Part 2: {minTotalWays[minTotalWays.Min(kvp => kvp.Key)]}");
    }

    public static void Day18(string[] data)
    {
        var width = data.First().Length;

        var grid = Enumerable.Repeat(false, width * width).ToArray();

        foreach (var line in data.WithIndex())
        {
            foreach (var character in line.Item.WithIndex())
            {
                grid[character.Index + line.Index * width] = character.Item == '#';
            }
        }

        foreach (var step in Enumerable.Range(0, 100))
        {
            var gridCopy = Enumerable.Repeat(false, width * width).ToArray();
            Array.Copy(grid, gridCopy, grid.Length);

            foreach (var y in Enumerable.Range(0, width))
            {
                foreach (var x in Enumerable.Range(0, width))
                {
                    //constraining neighbors to grid
                    var xMinus = x - 1 >= 0 ? x - 1 : -width * width;
                    var xPlus = x + 1 < width ? x + 1 : -width * width;
                    var yMinus = y - 1 >= 0 ? y - 1 : -width * width;
                    var yPlus = y + 1 < width ? y + 1 : -width * width;

                    var neighbors = new bool[]{
                        grid.ElementAtOrDefault(xMinus + yMinus * width), //top left
                        grid.ElementAtOrDefault(x      + yMinus * width), //top mid
                        grid.ElementAtOrDefault(xPlus  + yMinus * width), //top right
                        grid.ElementAtOrDefault(xMinus + y      * width), //mid left
                        grid.ElementAtOrDefault(xPlus  + y      * width), //mid right
                        grid.ElementAtOrDefault(xMinus + yPlus  * width), //bot left
                        grid.ElementAtOrDefault(x      + yPlus  * width), //bot mid
                        grid.ElementAtOrDefault(xPlus  + yPlus  * width), //bot right
                    };

                    var location = x + y * width;

                    var state = grid[location];

                    if (state && neighbors.Count(light => light) != 2 && neighbors.Count(light => light) != 3)
                    {
                        gridCopy[location] = false;
                    }

                    if (!state && neighbors.Count(light => light) == 3)
                    {
                        gridCopy[location] = true;
                    }
                }
            }

            grid = gridCopy;
        }

        Console.WriteLine($"Part 1: {grid.Count(light => light)}");

        grid = Enumerable.Repeat(false, width * width).ToArray();

        foreach (var line in data.WithIndex())
        {
            foreach (var character in line.Item.WithIndex())
            {
                grid[character.Index + line.Index * width] = character.Item == '#';
            }
        }

        foreach (var step in Enumerable.Range(0, 100))
        {
            grid[0] = true; //top left
            grid[width - 1] = true; //top right
            grid[(width - 1) * width] = true; //bottom left
            grid[(width - 1) + (width - 1) * width] = true; //bottom right

            var gridCopy = Enumerable.Repeat(false, width * width).ToArray();
            Array.Copy(grid, gridCopy, grid.Length);

            foreach (var y in Enumerable.Range(0, width))
            {
                foreach (var x in Enumerable.Range(0, width))
                {
                    //constraining neighbors to grid
                    var xMinus = x - 1 >= 0 ? x - 1 : -width * width;
                    var xPlus = x + 1 < width ? x + 1 : -width * width;
                    var yMinus = y - 1 >= 0 ? y - 1 : -width * width;
                    var yPlus = y + 1 < width ? y + 1 : -width * width;

                    var neighbors = new bool[]{
                        grid.ElementAtOrDefault(xMinus + yMinus * width), //top left
                        grid.ElementAtOrDefault(x      + yMinus * width), //top mid
                        grid.ElementAtOrDefault(xPlus  + yMinus * width), //top right
                        grid.ElementAtOrDefault(xMinus + y      * width), //mid left
                        grid.ElementAtOrDefault(xPlus  + y      * width), //mid right
                        grid.ElementAtOrDefault(xMinus + yPlus  * width), //bot left
                        grid.ElementAtOrDefault(x      + yPlus  * width), //bot mid
                        grid.ElementAtOrDefault(xPlus  + yPlus  * width), //bot right
                    };

                    var location = x + y * width;

                    var state = grid[location];

                    if (state && neighbors.Count(light => light) != 2 && neighbors.Count(light => light) != 3)
                    {
                        gridCopy[location] = false;
                    }

                    if (!state && neighbors.Count(light => light) == 3)
                    {
                        gridCopy[location] = true;
                    }
                }
            }

            gridCopy[0] = true; //top left
            gridCopy[width - 1] = true; //top right
            gridCopy[(width - 1) * width] = true; //bottom left
            gridCopy[(width - 1) + (width - 1) * width] = true; //bottom right

            grid = gridCopy;

            // var splitGrid = grid.Select((item, index) => new { Index = index, Value = item })
            //                     .GroupBy(item => item.Index / width)
            //                     .Select(item => item.Select(v => v.Value).ToList())
            //                     .ToList();

            // foreach (var line in splitGrid)
            // {
            //     foreach (var light in line)
            //     {
            //         Console.Write(light ? "#" : ".");
            //     }
            //     Console.Write("\n");
            // }
            // Console.Write("\n==================\n");
        }

        Console.WriteLine($"Part 2: {grid.Count(light => light)}");
    }

    public static void Day19(string[] data)
    {
        //key will be the replacement and value is the replaced
        var maps = new Dictionary<string, string>();
        var molecule = "";
        var replacementMolecules = new HashSet<string>();

        foreach (var line in data)
        {
            //last line is the medicine molecule
            if (line == data.Last())
            {
                molecule = line;
            }
            else
            {
                maps.Add(line.Split(" => ")[1], line.Split(" => ")[0]);
            }
        }

        var mapGroups = maps.GroupBy(kvp => kvp.Value);

        foreach (var mapGroup in mapGroups)
        {
            var moleculeToReplace = mapGroup.Key;

            foreach (var replacement in mapGroup)
            {
                var match = Regex.Match(molecule, moleculeToReplace);

                while (match.Success)
                {
                    var capture = match.Captures.First();
                    var newMolecule = molecule.Remove(capture.Index, capture.Length).Insert(capture.Index, replacement.Key);
                    replacementMolecules.Add(newMolecule);
                    match = match.NextMatch();
                }
            }
        }

        Console.WriteLine($"Part 1: {replacementMolecules.Count}");

        //seems like you need to know some logic and/or more advanced algorithms
        // shamelessly stealing logic from top comment of reddit solution thread
        // https://www.reddit.com/r/adventofcode/comments/3xflz8/comment/cy4etju/?utm_source=share&utm_medium=web2x&context=3

        //I think some before this one haven't *required* code but this one seemed (to me) to require some understanding
        // of the world that I don't have.
        var elements = Regex.Matches(molecule, "[A-Z][a-z]?").Count;
        var radon = Regex.Matches(molecule, "Rn").Count;
        var argon = Regex.Matches(molecule, "Ar").Count;
        var yttrium = Regex.Matches(molecule, "Y").Count;

        var answer = elements - (radon + argon) - (yttrium * 2) - 1;
        Console.WriteLine($"Part 2: {answer}");
    }

    public static void Day20(string[] data)
    {
        //I should probably stop doing these so late.
        // stuff that would probably be obvious to me 
        // is not so and I'm looking at answers to see
        // where I've gone wrong.

        var house = 1;
        var numToFind = int.Parse(data[0]);

        while (true)
        {
            var sum = GetDivisors(house).Sum() * 10;

            if (sum >= numToFind)
                break;

            house++;
        }

        Console.WriteLine($"Part 1: {house}");

        house = 1;

        while (true)
        {
            var sum = GetDivisors(house).Where(elf => house / elf <= 50).Sum() * 11;

            if (sum >= numToFind)
                break;

            house++;
        }

        Console.WriteLine($"Part 2: {house}");
    }

    public static void Day21(string[] data)
    {
        var bossHp = int.Parse(Regex.Matches(data[0], @"\d+").First().Groups[0].Value);
        var bossDmg = int.Parse(Regex.Matches(data[1], @"\d+").First().Groups[0].Value);
        var bossArm = int.Parse(Regex.Matches(data[2], @"\d+").First().Groups[0].Value);
        var playerHp = 100;

        var store = new List<RPGItem>{
            new RPGItem{Name = "Dagger",     Cost = 8,  Damage = 4, Armor = 0, Type = "Weapon"},
            new RPGItem{Name = "Shortsword", Cost = 10, Damage = 5, Armor = 0, Type = "Weapon"},
            new RPGItem{Name = "Warhammer",  Cost = 25, Damage = 6, Armor = 0, Type = "Weapon"},
            new RPGItem{Name = "Longsword",  Cost = 40, Damage = 7, Armor = 0, Type = "Weapon"},
            new RPGItem{Name = "Greataxe",   Cost = 74, Damage = 8, Armor = 0, Type = "Weapon"},

            new RPGItem{Name = "No Armor",   Cost = 0,   Armor = 0, Damage = 0, Type = "Armor"},
            new RPGItem{Name = "Leather",    Cost = 13,  Armor = 1, Damage = 0, Type = "Armor"},
            new RPGItem{Name = "Chainmail",  Cost = 31,  Armor = 2, Damage = 0, Type = "Armor"},
            new RPGItem{Name = "Splintmail", Cost = 53,  Armor = 3, Damage = 0, Type = "Armor"},
            new RPGItem{Name = "Bandedmail", Cost = 75,  Armor = 4, Damage = 0, Type = "Armor"},
            new RPGItem{Name = "Platemail",  Cost = 102, Armor = 5, Damage = 0, Type = "Armor"},

            new RPGItem{Name = "No Ring",    Cost = 0,   Damage = 0, Armor = 0, Type = "Ring"},
            new RPGItem{Name = "Damage +1",  Cost = 25,  Damage = 1, Armor = 0, Type = "Ring"},
            new RPGItem{Name = "Damage +2",  Cost = 50,  Damage = 2, Armor = 0, Type = "Ring"},
            new RPGItem{Name = "Damage +3",  Cost = 100, Damage = 3, Armor = 0, Type = "Ring"},
            new RPGItem{Name = "Defense +1", Cost = 20,  Armor = 1, Damage = 0, Type = "Ring"},
            new RPGItem{Name = "Defense +2", Cost = 40,  Armor = 2, Damage = 0, Type = "Ring"},
            new RPGItem{Name = "Defense +3", Cost = 80,  Armor = 3, Damage = 0, Type = "Ring"}
        };

        //WithRepetition will allow for optional rings and armor
        var purchaseOpportunities = new Combinations<RPGItem>(store, 4, GenerateOption.WithRepetition)
                                                .Where(items => items.Count(item => item.Type == "Ring") < 3
                                                             && items.Count(item => item.Type == "Armor") < 2
                                                             && items.Count(item => item.Type == "Weapon") == 1);

        var optimalPurchase = purchaseOpportunities.Where(items =>
            {
                var playerDamage = items.DistinctBy(item => item.Name).Sum(item => item.Damage);
                var playerArmor = items.DistinctBy(item => item.Name).Sum(item => item.Armor);

                var playerTurns = Math.Ceiling(bossHp / (double)Math.Max(playerDamage - bossArm, 1));
                var bossTurns = Math.Ceiling(playerHp / (double)Math.Max(bossDmg - playerArmor, 1));

                return playerTurns <= bossTurns;
            })
            .OrderBy(items => items.DistinctBy(item => item.Name).Sum(item => item.Cost))
            .First();

        Console.WriteLine($"Part 1: {optimalPurchase.Sum(item => item.Cost)}");

        var inoptimalPurchase = purchaseOpportunities.Where(items =>
            {
                var playerDamage = items.DistinctBy(item => item.Name).Sum(item => item.Damage);
                var playerArmor = items.DistinctBy(item => item.Name).Sum(item => item.Armor);

                var playerTurns = Math.Ceiling(bossHp / (double)Math.Max(playerDamage - bossArm, 1));
                var bossTurns = Math.Ceiling(playerHp / (double)Math.Max(bossDmg - playerArmor, 1));

                return playerTurns > bossTurns;
            })
            .OrderByDescending(items => items.DistinctBy(item => item.Name).Sum(item => item.Cost))
            .First();

        Console.WriteLine($"Part 2: {inoptimalPurchase.Sum(item => item.Cost)}");
    }

    public static void Day22(string[] data)
    {
        /*
            Apparently this is solvable but I got the most difficult input.
            This was the most difficult one I've worked on so far. My mind just
            wouldn't work with it. The order of events is super important and
            the text on the page isn't necessarily how it's implemented

            https://www.reddit.com/r/adventofcode/comments/3xspyl/day_22_solutions/cy86y2x/
            boss = { hp: 71, damage: 10 }
            player = { hp: 50, mana: 500 }

            hard: 1937
            * Shield -> Recharge -> Poison -> Shield -> Recharge -> Poison -> Shield -> Recharge -> Poison -> Shield -> Magic Missile -> Poison -> Magic Missile

            easy: 1824
            * Poison -> Recharge -> Shield -> Poison -> Recharge -> Shield -> Poison -> Recharge -> Shield -> Magic Missile -> Poison -> Magic Missile
            * Poison -> Recharge -> Shield -> Poison -> Recharge -> Shield -> Poison -> Recharge -> Shield -> Poison -> Magic Missile -> Magic Missile
            * Recharge -> Poison -> Shield -> Recharge -> Poison -> Shield -> Recharge -> Poison -> Shield -> Magic Missile -> Poison -> Magic Missile

            I can't cast spells at random and hope I get the right combination like other solutions can
        */
        var minMana = int.MaxValue;

        var playerSpells = new List<Spell>{
                new Spell{ Name = "Magic Missile", Cost = 53,  Effect = new Effect{             Damage = 4 } },
                new Spell{ Name = "Drain",         Cost = 73,  Effect = new Effect{ Damage = 2, Heal = 2   } },
                new Spell{ Name = "Shield",        Cost = 113, Effect = new Effect{ Time = 6,   Armor = 7  } },
                new Spell{ Name = "Poison",        Cost = 173, Effect = new Effect{ Time = 6,   Damage = 3 } },
                new Spell{ Name = "Recharge",      Cost = 229, Effect = new Effect{ Time = 5,   Mana = 101 } }
        };

        var player = new Player
        {
            Mana = 500,
            Health = 50,
            Spells = playerSpells
        };

        var boss = new Player{
            Health = int.Parse(Regex.Matches(data[0], @"\d+").First().Groups[0].Value),
            Damage = int.Parse(Regex.Matches(data[1], @"\d+").First().Groups[0].Value)
        };
        
        Action<Player, Player> ResolveEffects = (player, boss) =>
        {
            var shield = player.Spells.Single(spell => spell.Name == "Shield");
            var poison = player.Spells.Single(spell => spell.Name == "Poison");
            var recharge = player.Spells.Single(spell => spell.Name == "Recharge");            

            boss.Health -= poison.Effect.Active ? poison.Effect.Damage : 0;
            player.Mana += recharge.Effect.Active ? recharge.Effect.Mana : 0;
            player.Armor = shield.Effect.Active ? shield.Effect.Armor : 0;

            if (shield.Effect.Active)
            {
                shield.Effect.Tick += 1;

                if (shield.Effect.Tick == shield.Effect.Time)
                {
                    shield.Effect.Active = false;
                    shield.Effect.Tick = 0;
                }
            }

            if (recharge.Effect.Active)
            {
                recharge.Effect.Tick += 1;

                if (recharge.Effect.Tick == recharge.Effect.Time)
                {
                    recharge.Effect.Active = false;
                    recharge.Effect.Tick = 0;
                }
            }

            if (poison.Effect.Active)
            {
                poison.Effect.Tick += 1;

                if (poison.Effect.Tick == poison.Effect.Time)
                {
                    poison.Effect.Active = false;
                    poison.Effect.Tick = 0;
                }
            }

        };

        Action<Player, Player, bool> Fight = null;

        Fight = (player, boss, hardMode) =>
        {
            foreach (var spell in player.Spells.Where(spell => !spell.Effect.Active 
                                                        && spell.Cost <= player.Mana
                                                        && spell.Cost + player.ManaSpent < minMana))
            {
                var newPlayer = player.Copy();
                var newBoss = boss.Copy();         
                
                #region Player
                
                if(hardMode)
                    newPlayer.Health -= 1;

                newPlayer.Mana -= spell.Cost;
                newPlayer.ManaSpent += spell.Cost;

                if (spell.Name == "Poison" || spell.Name == "Shield" || spell.Name == "Recharge")
                    newPlayer.Spells.Single(newSpell => newSpell.Name == spell.Name).Effect.Active = true;

                if (spell.Name == "Drain")
                {
                    newPlayer.Health += spell.Effect.Heal;
                    newBoss.Health -= spell.Effect.Damage;
                }

                if (spell.Name == "Magic Missile")
                {
                    newBoss.Health -= spell.Effect.Damage;
                }

                ResolveEffects(newPlayer, newBoss);

                if (newBoss.Health <= 0)
                {
                    minMana = Math.Min(newPlayer.ManaSpent, minMana);
                    break;
                }               

                #endregion              
                ResolveEffects(newPlayer, newBoss);

                if (newBoss.Health <= 0)
                {
                    minMana = Math.Min(newPlayer.ManaSpent, minMana);
                    break;
                }

                newPlayer.Health -= Math.Max(newBoss.Damage - newPlayer.Armor, 1);

                if (newPlayer.Health > 0)
                {
                    Fight(newPlayer, newBoss, hardMode);
                }
            }
        };

        Fight(player, boss, false);

        Console.WriteLine($"Part 1: {minMana}");

        minMana = int.MaxValue;
        Fight(player, boss, true);

        Console.WriteLine($"Part 2: {minMana}");
    }

    public static void Day23(string[] data){
        //initially was using int but part 2 was causing an overflow
        var registers = new Dictionary<string, long>() {{"a" , 0}, {"b", 0}};

        Func<long, long> HLF = register => { return register >> 1;};
        Func<long, long> TPL = register => { return register * 3;};
        Func<long, long> INC = register => { return register + 1;};
        Func<long, bool> JIE = register => { return register % 2 == 0;};
        Func<long, bool> JIO = register => { return register == 1;};

        for(int i = 0; i < data.Length; i++)
        {
            var instruction = data[i].Substring(0, 3);
            var register = "";
            var offset = 0;

            if(instruction != "jmp")
                register = data[i].Substring(4, 1);

            if(instruction == "jmp" || instruction == "jie" || instruction == "jio")
                offset = int.Parse(Regex.Matches(data[i], @"((\+|-)\d+)").First().Groups[0].Value);

            switch(instruction)
            {                
                case "hlf":
                    registers[register] = HLF(registers[register]);
                break;

                case "tpl":
                    registers[register] = TPL(registers[register]);
                break;

                case "inc":
                    registers[register] = INC(registers[register]);
                break;

                case "jmp":
                    i += offset - 1;
                break;

                case "jie":
                    i += JIE(registers[register]) ? offset - 1 : 0;
                break;

                case "jio":
                    i += JIO(registers[register]) ? offset - 1 : 0;
                break;
            }
        }
    
        Console.WriteLine(registers["b"]);

        registers = new Dictionary<string, long>() {{"a" , 1}, {"b", 0}};

        for(int i = 0; i < data.Length; i++)
        {
            var instruction = data[i].Substring(0, 3);
            var register = "";
            var offset = 0;

            if(instruction != "jmp")
                register = data[i].Substring(4, 1);

            if(instruction == "jmp" || instruction == "jie" || instruction == "jio")
                offset = int.Parse(Regex.Matches(data[i], @"((\+|-)\d+)").First().Groups[0].Value);

            switch(instruction)
            {                
                case "hlf":
                    registers[register] = HLF(registers[register]);
                break;

                case "tpl":
                    registers[register] = TPL(registers[register]);
                break;

                case "inc":
                    registers[register] = INC(registers[register]);
                break;

                //offset - 1 since we will increment the offset at the beginning of the loop
                case "jmp":
                    i += offset - 1;
                break;

                case "jie":
                    i += JIE(registers[register]) ? offset - 1 : 0;
                break;

                case "jio":
                    i += JIO(registers[register]) ? offset - 1 : 0;
                break;

                default:
                throw new Exception("Invalid instruction");
            }
        }

         Console.WriteLine(registers["b"]);
    }
}
