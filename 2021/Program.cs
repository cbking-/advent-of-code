using System.Diagnostics;
using System.Reflection;
using System.Text;
using Core;
using MathNet.Numerics.Statistics;
using static Core.Helpers;

var data = await LoadDataAsync(args[0]);

var adventType = typeof(AdventOfCode);
var dayToRun = adventType.GetMethod(args[0], BindingFlags.Static | BindingFlags.Public) ?? throw new ArgumentException("Invalid day");
dayToRun.Invoke(null, new object[] { data });

public static class AdventOfCode
{
    #region Objects
    public class BingoBoard
    {
        public List<BingoPlace> Places { get; set; } = new List<BingoPlace>();

        public int BoardWidth { get; set; } = 0;

        public bool HasBingo()
        {
            //check rows
            foreach (var row in Enumerable.Range(0, BoardWidth))
            {
                var MarkedCount = 0;

                foreach (var column in Enumerable.Range(0, BoardWidth))
                {
                    MarkedCount += Places[column + row * BoardWidth].Marked ? 1 : 0;
                }

                if (MarkedCount == BoardWidth)
                    return true;
            }

            //check columns
            foreach (var column in Enumerable.Range(0, BoardWidth))
            {
                var MarkedCount = 0;

                foreach (var row in Enumerable.Range(0, BoardWidth))
                {
                    MarkedCount += Places[column + row * BoardWidth].Marked ? 1 : 0;
                }

                if (MarkedCount == BoardWidth)
                    return true;
            }

            return false;
        }
    }

    [DebuggerDisplay("{Number} {Marked}")]
    public class BingoPlace
    {
        public bool Marked { get; set; } = false;
        public int Number { get; set; } = 0;
    }

    public class Packet
    {
        private string Payload = string.Empty;
        public int Version = 0;
        private int TypeId = 0;
        private int LengthTypeId = 0;
        public long Value = 0;
        private List<Packet> SubPackets = new List<Packet>();
        public int BitLength = 6;

        public Packet(string _payload)
        {
            Payload = _payload;

            var offset = 0;

            Version = Convert.ToInt32(Payload.Substring(offset, 3), 2);
            offset += 3;

            TypeId = Convert.ToInt32(Payload.Substring(offset, 3), 2);
            offset += 3;

            LengthTypeId = Convert.ToInt32(Payload.Substring(offset, 1), 2);

            if (TypeId != 4)
            {
                if (LengthTypeId == 0)
                {
                    offset += 1;

                    var subPacketLength = Convert.ToInt32(Payload.Substring(offset, 15), 2);
                    offset += 15;

                    var parsedLength = 0;

                    while (parsedLength < subPacketLength)
                    {
                        var subPacket = new Packet(Payload[offset..]);
                        offset += subPacket.BitLength;
                        parsedLength += subPacket.BitLength;

                        SubPackets.Add(subPacket);
                    }

                    BitLength += 16 + SubPackets.Sum(packet => packet.BitLength);
                }
                else if (LengthTypeId == 1)
                {
                    offset += 1;

                    var numberOfSubPackets = Convert.ToInt32(Payload.Substring(offset, 11), 2);
                    offset += 11;

                    while (SubPackets.Count < numberOfSubPackets)
                    {
                        var subPacket = new Packet(Payload[offset..]);
                        offset += subPacket.BitLength;

                        SubPackets.Add(subPacket);
                    }

                    BitLength += 12 + SubPackets.Sum(packet => packet.BitLength);
                }
            }

            else
                Value = ParseLiteralValue(Payload.Substring(offset));

            Value = Evaluate();
        }

        public int GetVersionSum()
        {
            if (SubPackets.Count == 0)
                return Version;

            return Version + SubPackets.Sum(packet => packet.GetVersionSum());
        }

        public long Evaluate()
        {
            switch (TypeId)
            {
                case 0:
                    return SubPackets.Sum(packet => packet.Value);
                case 1:
                    return SubPackets.Aggregate((long)1, (total, item) => total * item.Value);
                case 2:
                    return SubPackets.Min(packet => packet.Value);
                case 3:
                    return SubPackets.Max(packet => packet.Value);
                case 4:
                    return Value;
                case 5:
                    return SubPackets.First().Value > SubPackets.Skip(1).First().Value ? 1 : 0;
                case 6:
                    return SubPackets.First().Value < SubPackets.Skip(1).First().Value ? 1 : 0;
                case 7:
                    return SubPackets.First().Value == SubPackets.Skip(1).First().Value ? 1 : 0;
                default:
                    return 0;
            }
        }

        private long ParseLiteralValue(string message)
        {
            var literalValue = "";

            //batches in max of 5
            // last batch should return less than 5 if padded
            foreach (var number in message.Batch(5))
            {
                BitLength += 5;

                var binary = string.Join("", number);

                literalValue += binary.Substring(1);

                //last group
                if (number.First() == '0')
                {
                    break;
                }
            }

            return Convert.ToInt64(literalValue, 2);
        }
    }

    [DebuggerDisplay("{Left} {Right}")]
    public class Pair
    {
        public Pair? Parent {get; set;}
        public dynamic? Left {get; set;}
        public dynamic? Right {get; set;}

        public override string ToString()
        {
            return $"[{Left},{Right}]";
        }
    }
    #endregion

    #region Completed
    public static void Day1(string[] data)
    {
        var numbers = Array.ConvertAll(data, line => int.Parse(line));

        //skip the first one since there's nothing to compare it to
        // Calling Skip(x) will be zero indexed (creates a new list) but pass that index to the
        // data list to get the previous value from the current one from the Skip list.
        // Generates list of true or false then count the true values.
        Console.WriteLine($"Part 1: {numbers.Skip(1).Select((value, index) => value > numbers[index]).Count(value => value)}");

        //skip the first three since there's nothing to compare them to
        // since two windows share two of the same numbers, we don't need to calculate any sums
        // only compare the first number of the first window and the last number of the second window
        Console.WriteLine($"Part 2: {numbers.Skip(3).Select((value, index) => value > numbers[index]).Count(value => value)}");

        //Another option (and maybe makes more sense) is to use Zip as we don't rely on knowing Skip is indexed zero
        // Console.WriteLine($"Part 1: {numbers.Zip(numbers.Skip(1), (first, second) => first < second).Count(value => value)}");
        // Console.WriteLine($"Part 2: {numbers.Zip(numbers.Skip(3), (first, second) => first < second).Count(value => value)}");

        //Aggregate is an option here as well though I don't like it as much as Zip
        // Console.WriteLine($@"Part 1: {numbers.Skip(1).Select((Value, Index) => new {Value, Index})
        //                                              .Aggregate(0, (total, line) => numbers[line.Index] < line.Value ? total += 1 : total)}");
        // Console.WriteLine($@"Part 2: {numbers.Skip(3).Select((Value, Index) => new {Value, Index})
        //                                              .Aggregate(0, (total, line) => numbers[line.Index] < line.Value ? total += 1 : total)}");
    }

    public static void Day2(string[] data)
    {
        //Initial solutions were using regex to parse and foreach loops Linq is
        // a little more elegant and Regex was a bit overkill

        //I don't think group by can be used here since
        // forward uses the current cumulative value of aim per iteration
        var calculation = data.Aggregate((0, 0, 0),
            (accumulator, line) =>
            {
                var split = line.Split(' ');

                if (split[0] == "down")
                {
                    accumulator.Item1 += int.Parse(split[1]);
                }

                if (split[0] == "up")
                {
                    accumulator.Item1 -= int.Parse(split[1]);
                }

                if (split[0] == "forward")
                {
                    accumulator.Item2 += int.Parse(split[1]);
                    accumulator.Item3 += accumulator.Item1 * int.Parse(split[1]);
                }

                return accumulator;
            });

        Console.WriteLine($"Part 1: {calculation.Item2 * calculation.Item1}");
        Console.WriteLine($"Part 2: {calculation.Item2 * calculation.Item3}");
    }

    public static void Day3(string[] data)
    {
        var gammaRate = new StringBuilder();
        var epsilonRate = new StringBuilder();

        foreach (var index in Enumerable.Range(0, data[0].Length))
        {
            var groups = data.GroupBy(line => line.ElementAt(index));
            gammaRate.Append(groups.OrderBy(group => group.Count()).First().Key);
            epsilonRate.Append(groups.OrderByDescending(group => group.Count()).First().Key);
        }

        Console.WriteLine($"Part 1: {Convert.ToInt32(gammaRate.ToString(), 2) * Convert.ToInt32(epsilonRate.ToString(), 2)}");

        var diagnosticData = new List<String>(data);
        var oxyGenRate = new List<string>(data);
        var co2ScrubRate = new List<string>(data);

        foreach (var index in Enumerable.Range(0, data[0].Length))
        {
            var groups = diagnosticData.GroupBy(line => line.ElementAt(index));
            oxyGenRate = oxyGenRate.Intersect(groups.OrderByDescending(group => group.Count())
                                                    .ThenByDescending(group => group.Key)
                                                    .First()
                                                    .Select(group => group))
                                                .ToList();
            diagnosticData = diagnosticData.Intersect(oxyGenRate).ToList();
        }

        diagnosticData = new List<String>(data);

        foreach (var index in Enumerable.Range(0, data[0].Length))
        {
            var groups = diagnosticData.GroupBy(line => line.ElementAt(index));
            co2ScrubRate = co2ScrubRate.Intersect(groups.OrderBy(group => group.Count())
                                                        .ThenBy(group => group.Key)
                                                        .First()
                                                        .Select(group => group))
                                                    .ToList();
            diagnosticData = diagnosticData.Intersect(co2ScrubRate).ToList();
        }

        Console.WriteLine($"Part 2: {Convert.ToInt32(oxyGenRate.Single(), 2) * Convert.ToInt32(co2ScrubRate.Single(), 2)}");
    }

    public static void Day4(string[] data)
    {
        var draws = Array.ConvertAll(data[0].Split(','), int.Parse);
        var partOne = 0;
        var partTwo = 0;
        var boards = new List<BingoBoard>();
        var boardWidth = 5;
        var board = new BingoBoard() { BoardWidth = boardWidth };

        foreach (var line in data.WithIndex().Skip(1))
        {
            var row = Array.ConvertAll(line.Item.Split(' ', StringSplitOptions.RemoveEmptyEntries), number => int.Parse(number));

            foreach (var number in row.WithIndex())
            {
                var location = (number.Index % 5) + ((line.Index % 5) - 1) * boardWidth;
                board.Places.Add(new BingoPlace { Number = number.Item });
            }

            if (line.Index % boardWidth == 0)
            {
                boards.Add(board);
                board = new BingoBoard() { BoardWidth = boardWidth };
            }
        }

        foreach (var draw in draws)
        {
            boards.ForEach(board => board.Places.Where(place => place.Number == draw).ToList().ForEach(place => place.Marked = true));

            if (boards.Any(board => board.HasBingo()))
            {
                if (partOne == 0)
                    partOne = boards.Where(board => board.HasBingo()).Single().Places.Where(place => !place.Marked).Sum(place => place.Number) * draw;

                if (boards.Count() == 1)
                    partTwo = boards.Single().Places.Where(place => !place.Marked).Sum(place => place.Number) * draw;

                boards.RemoveAll(board => board.HasBingo());
            }
        }

        Console.WriteLine($"Part 1: \x1b[93m{partOne}\x1b[0m");
        Console.WriteLine($"Part 2: \x1b[93m{partTwo}\x1b[0m");
    }

    public static void Day5(string[] data)
    {
        //There are some tuple swaps you can do
        // when comparing coordinates so the for loops are more condensed
        // E.g. for diagonals
        //   if(xStart > xEnd)
        //      ((xStart, yStart), (xEnd, yEnd)) = ((xEnd, yEnd), (xStart, yStart))
        //
        // Could also use a HashSet instead of a List and
        // increment some count when HashSet.Add returns false

        var straightVents = new List<(int, int)>();
        var diagVents = new List<(int, int)>();

        foreach (var set in data)
        {
            var start = set.Split(" -> ")[0];
            var end = set.Split(" -> ")[1];

            var xStart = int.Parse(start.Split(',')[0]);
            var yStart = int.Parse(start.Split(',')[1]);
            var xEnd = int.Parse(end.Split(',')[0]);
            var yEnd = int.Parse(end.Split(',')[1]);

            var steps = Math.Abs(xStart - xEnd) + 1;

            if (xStart == xEnd)
            {
                foreach (var yCoord in Enumerable.Range(Math.Min(yStart, yEnd), Math.Abs(yStart - yEnd) + 1))
                {
                    straightVents.Add((xStart, yCoord));
                }
            }
            else if (yStart == yEnd)
            {
                foreach (var xCoord in Enumerable.Range(Math.Min(xStart, xEnd), Math.Abs(xStart - xEnd) + 1))
                {
                    straightVents.Add((xCoord, yStart));
                }
            }
            else if (xStart < xEnd && yStart < yEnd)
            {
                //starting top left
                foreach (var step in Enumerable.Range(0, steps))
                {
                    diagVents.Add((xStart + step, yStart + step));
                }
            }
            else if (xStart > xEnd && yStart < yEnd)
            {

                //starting top right
                foreach (var step in Enumerable.Range(0, steps))
                {
                    diagVents.Add((xStart - step, yStart + step));
                }
            }
            else if (xStart < xEnd && yStart > yEnd)
            {
                //starting bottom left
                foreach (var step in Enumerable.Range(0, steps))
                {
                    diagVents.Add((xStart + step, yStart - step));
                }
            }
            else if (xStart > xEnd && yStart > yEnd)
            {
                //starting bottom right
                foreach (var step in Enumerable.Range(0, steps))
                {
                    diagVents.Add((xStart - step, yStart - step));
                }
            }
        }

        var partOne = straightVents.GroupBy(vent => vent).Where(vent => vent.Count() > 1).Count();

        Console.WriteLine($"Part 1: \x1b[93m{partOne}\x1b[0m");

        var partTwo = straightVents.Concat(diagVents).GroupBy(vent => vent).Where(vent => vent.Count() > 1).Count();

        Console.WriteLine($"Part 2: \x1b[93m{partTwo}\x1b[0m");
    }

    public static void Day6(string[] data)
    {
        //First solution was to create a list of integers
        // where each integer was a fish's countdown. Then
        // Add to the list once it reached zero and add one to 8 and
        // reset the current fish to 6.
        // This worked for the part one but exponential growth kills part two.
        // This solution keeps track of the number of fish in each day and the new fishes
        // that are generated. The growth is tracked through longs rather than
        // the size of a List. I keep track of new fish in a separate array
        // so they aren't overwritten as the other fish that haven't spawned
        // are figured out. I'm going to start using a time in the interest
        // of seeing how quick my solutions are and if certain implementations
        // are quicker than others.

        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        var fishes = new long[9];

        Array.ConvertAll(data[0].Split(','), int.Parse).ToList().ForEach(state => fishes[state] += 1);

        foreach (var day in Enumerable.Range(1, 80))
        {
            var nextFishes = new long[9];
            var nextNewFishes = new long[9];

            Array.Copy(fishes, nextFishes, fishes.Length);

            foreach (var group in fishes.WithIndex())
            {
                if (group.Index == 0)
                {
                    nextNewFishes[8] += fishes[0];
                    nextNewFishes[6] += fishes[0];
                    nextFishes[0] = 0;
                }
                else
                {
                    nextFishes[group.Index - 1] += fishes[group.Index];
                    nextFishes[group.Index] = nextNewFishes[group.Index]; //this will put in the sixes and eights
                }
            }

            Array.Copy(nextFishes, fishes, nextFishes.Length);
        }

        Console.WriteLine($"Part 1: \x1b[93m{fishes.Sum()}\x1b[0m");

        watch.Stop();
        Console.WriteLine($"Part 1 Execution Time: {watch.ElapsedMilliseconds} ms");

        watch.Reset();
        watch.Start();

        //continue to the 256th day
        foreach (var day in Enumerable.Range(1, 176))
        {
            var nextFishes = new long[9];
            var nextNewFishes = new long[9];

            Array.Copy(fishes, nextFishes, fishes.Length);

            foreach (var group in fishes.WithIndex())
            {
                if (group.Index == 0)
                {
                    nextNewFishes[8] += fishes[0];
                    nextNewFishes[6] += fishes[0];
                    nextFishes[0] = 0;
                }
                else
                {
                    nextFishes[group.Index - 1] += fishes[group.Index];
                    nextFishes[group.Index] = nextNewFishes[group.Index]; //this will put in the sixes and eights
                }
            }

            Array.Copy(nextFishes, fishes, nextFishes.Length);
        }

        Console.WriteLine($"Part 2: \x1b[93m{fishes.Sum()}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 2 Execution Time: {watch.ElapsedMilliseconds} ms");
    }

    public static void Day7(string[] data)
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        //double since we will be caclulating the median which could be a double value
        // Though this is unlikely as AOC only deals with integers but that's how
        // the library implements it
        var positions = Array.ConvertAll(data[0].Split(',', StringSplitOptions.RemoveEmptyEntries), double.Parse);

        //added a whole package for getting the median but their implementation is going to be way better
        // than anything I would implement (copy + paste from stackoverflow)
        var positionToMoveTo = positions.Median();

        Console.WriteLine($"Part 1: \x1b[93m{positions.Aggregate(0, (acc, position) => (int)Math.Abs(position - positionToMoveTo) + acc)}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 1 Execution Time: {watch.ElapsedMilliseconds} ms");

        watch.Reset();
        watch.Start();
        var bestMovement = int.MaxValue;

        foreach (var tryPosition in Enumerable.Range(0, (int)positions.Max()))
        {
            bestMovement = Math.Min(bestMovement, positions.Aggregate(0, (acc, position) =>
            {
                var n = (int)Math.Abs(position - tryPosition);
                return ((n * (n + 1)) / 2) + acc;
            }));
        }

        Console.WriteLine($"Part 2: \x1b[93m{bestMovement}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 2 Execution Time: {watch.ElapsedMilliseconds} ms");
    }

    public static void Day8(string[] data)
    {
        // original solution for part 1
        // I figured part two was going to ask to decode everything
        // but this is a super straightforward solution for part 1

        // var answer = data.Aggregate(0, (acc, line) =>{
        //     var output = line.Split('|', StringSplitOptions.RemoveEmptyEntries)[1]
        //                      .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        //     return output.Where(number => number.Length == 2 ||
        //                         number.Length == 3 ||
        //                         number.Length == 4 ||
        //                         number.Length == 7)
        //                  .Count() + acc;
        // });

        // Console.WriteLine(answer);
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        var sevenSegmentNumbers = new Dictionary<string, int>()
        {
            {"abcefg", 0 },
            {"cf", 1 },
            {"acdeg", 2 },
            {"acdfg", 3 },
            {"bcdf", 4 },
            {"abdfg", 5 },
            {"abdefg", 6 },
            {"acf", 7 },
            {"abcdefg", 8 },
            {"abcdfg", 9 }
        };

        var decoded = new List<string>();

        foreach (var line in data)
        {
            var map = new Dictionary<char, char>();

            var input = line.Split('|', StringSplitOptions.RemoveEmptyEntries)[0]
                            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var output = line.Split('|', StringSplitOptions.RemoveEmptyEntries)[1]
                            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            //1, 4, 7, 8 are unique so super easy to find
            var one = input.Where(ssd => ssd.Length == 2).Single();
            var four = input.Where(ssd => ssd.Length == 4).Single();
            var seven = input.Where(ssd => ssd.Length == 3).Single();
            var eight = input.Where(ssd => ssd.Length == 7).Single();

            //3 is the only 5 segment number that includes the same segments as 1
            var three = input.Where(ssd => one.All(ssd.Contains) && ssd.Length == 5).Single();

            //7 and 1 shared two segements in common so the third can be mapped
            map[seven.Except(one).Single()] = 'a';

            //3 has only one segment in common with 4 after removing 4's common segments with 1
            map[three.Intersect(four.Except(one)).Single()] = 'd';

            //we still don't know what the segements of 1 map to but we've figured out
            // one of the other two segments four has so map the other
            map[four.Except(one).Where(character => !map.ContainsKey(character)).Single()] = 'b';

            //now that we know the segment mapping for a, b, and d
            // we can figure out which input is 5 as it's the only five segment
            // display that has a, b, and d
            var five = input.Where(ssd => map.Keys.All(ssd.Contains) && ssd.Length == 5).Single();

            //5 shares one segment in common with 1 so we can map that one
            map[five.Intersect(one).Single()] = 'f';

            //map the last unmapped segment from 5
            map[five.Where(character => !map.ContainsKey(character)).Single()] = 'g';

            //we figured out one segment from 1 using 5 so map the other segment
            map[one.Where(character => !map.ContainsKey(character)).Single()] = 'c';

            //map the last segment
            map[eight.Where(character => !map.ContainsKey(character)).Single()] = 'e';

            //Add the decoded output using the map we build
            decoded.Add(output.Aggregate("", (acc, ssd) =>
            {
                var key = string.Join("", ssd.Select(character => map[character]).OrderBy(character => character));
                return acc + sevenSegmentNumbers[key].ToString();
            }));
        }

        var partOne = decoded.Aggregate(0, (acc, output) => acc + output.Aggregate(0, (acc, character) =>
        {
            return acc + (character == '1' || character == '4' || character == '7' || character == '8' ? 1 : 0);
        }));

        Console.WriteLine($"Part 1: \x1b[93m{partOne}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 1 Execution Time: {watch.ElapsedMilliseconds} ms");

        watch.Reset();
        watch.Start();
        var partTwo = decoded.Aggregate(0, (acc, output) => acc + int.Parse(output));

        Console.WriteLine($"Part 2: \x1b[93m{partTwo}\x1b[0m");

        watch.Stop();
        Console.WriteLine($"Part 2 Execution Time: {watch.ElapsedMilliseconds} ms");
    }

    public static void Day9(string[] data)
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        //looks weird but join all lines from the data then split into individual characters.
        // I like interacting with 2d arrays as a 1d array and using the x + y * width formula
        var width = data[0].Length;
        var map = string.Join("", data);

        var lowPoints = map.WithIndex().Select(point =>
        {
            var neighbors = new char[]{
                map.ElementAtOrDefault(point.Index % width == 0 ? -1 : point.Index - 1),     //left neighbor
                map.ElementAtOrDefault((point.Index + 1) % width == 0 ? -1 : point.Index + 1),     //right neighbor
                map.ElementAtOrDefault(point.Index - width), //top neighbor
                map.ElementAtOrDefault(point.Index + width)  //bottom neighbor
            };

            if (point.Item < neighbors.Where(character => character != 0).Min())
            {
                return point.Index;
            }

            return -1;
        }).Where(point => point != -1);

        var partOne = lowPoints.Aggregate(0, (acc, point) => int.Parse(map.ElementAtOrDefault(point).ToString()) + 1 + acc);

        Console.WriteLine($"Part 1: \x1b[93m{partOne}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 1 Execution Time: {watch.ElapsedMilliseconds} ms");

        watch.Reset();
        watch.Start();

        var partTwo = lowPoints.Select(point =>
        {
            var basin = new List<int>{
                point,
                point % width == 0 ? -1 : point - 1,
                (point + 1) % width == 0 ? -1 : point + 1,
                point - width,
                point + width
            };

            basin = basin.Where(location => map.ElementAtOrDefault(location) != '9' && map.ElementAtOrDefault(location) != 0).ToList();

            var searchIndexes = basin.AsEnumerable(); ;

            do
            {
                var initialIndexes = basin.AsEnumerable();

                var addToBasin = searchIndexes.SelectMany(index => new List<int>{
                                                                            index % width == 0 ? -1 : index - 1,
                                                                            (index + 1) % width == 0 ? -1 : index + 1,
                                                                            index - width,
                                                                            index + width
                                                                        })
                                              .Where(location => map.ElementAtOrDefault(location) != '9' && map.ElementAtOrDefault(location) != 0);

                basin = basin.Union(addToBasin).ToList();

                searchIndexes = basin.Except(initialIndexes);

            } while (searchIndexes.Count() > 0);

            return basin.Count();
        }).OrderByDescending(basin => basin).Take(3).Aggregate(1, (acc, basin) => basin * acc);

        Console.WriteLine($"Part 2: \x1b[93m{partTwo}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 2 Execution Time: {watch.ElapsedMilliseconds} ms");
    }

    public static void Day10(string[] data)
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        var chunckDefs = new Dictionary<char, char>(){
            {')' , '('},
            {']' , '['},
            {'}' , '{'},
            {'>' , '<'}
        };

        var pointsDef = new Dictionary<char, int>(){
            {')' , 3},
            {']' , 57},
            {'}' , 1197},
            {'>' , 25137}
        };

        var pointsDefTwo = new Dictionary<char, int>(){
            {')' , 1},
            {']' , 2},
            {'}' , 3},
            {'>' , 4}
        };

        var partOne = 0;
        var partTwoScores = new List<long>();

        foreach (var line in data)
        {
            var chunckStack = new Stack<char>();
            var corrupt = false;
            foreach (var character in line)
            {
                if (chunckStack.Count == 0 || chunckDefs.ContainsValue(character))
                {
                    chunckStack.Push(character);
                }
                else if (chunckDefs[character] == chunckStack.Peek())
                {
                    chunckStack.Pop();
                }
                else
                {
                    var key = chunckDefs.Where(kvp => kvp.Value == chunckStack.Peek()).Single().Key;
                    partOne += pointsDef[character];
                    //Console.WriteLine($"Expected {key}, but found {character} instead");
                    corrupt = true;
                }

                if (corrupt)
                    break;
            }

            if (!corrupt)
            {
                var finishers = chunckStack.Select(chunkStart => chunckDefs.Where(kvp => kvp.Value == chunkStart).Single().Key);
                var partTwo = finishers.Aggregate<char, long>(0, (acc, finisher) => (acc * 5) + pointsDefTwo[finisher]);
                partTwoScores.Add(partTwo);
            }
        }

        Console.WriteLine($"Part 1: \x1b[93m{partOne}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 1 Execution Time: {watch.ElapsedMilliseconds} ms");

        watch.Reset();
        watch.Start();
        Console.WriteLine($"Part 2: \x1b[93m{partTwoScores.OrderBy(score => score).Take((partTwoScores.Count() / 2) + 1).Last()}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 2 Execution Time: {watch.ElapsedMilliseconds} ms");
    }

    public static void Day11(string[] data)
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        //converting 1d array to 2d array because that's how I roll
        var width = data[0].Length;
        var octopi = string.Join("", data).ToCharArray();
        var flashCount = 0;
        var flashed = new List<int>();

        Action Draw = () =>
        {
            foreach (var y in Enumerable.Range(0, width))
            {
                foreach (var x in Enumerable.Range(0, width))
                {
                    if (octopi[x + y * width] == '0')
                    {
                        Console.Write($"\x1b[93m{octopi[x + y * width]}\x1b[0m");
                    }
                    else
                    {
                        Console.Write(octopi[x + y * width]);
                    }

                }
                Console.Write(Environment.NewLine);
            }
            Console.WriteLine("=================");
        };

        Action<int> Flash = (index) => { };

        Flash = (index) =>
        {
            flashCount += 1;
            flashed.Add(index);

            var left = index % width == 0 ? -1 : index - 1;
            var right = (index + 1) % width == 0 ? -1 : index + 1;
            var bottom = (index + width) >= octopi.Length ? -1 : index + width;
            var bottomLeft = left == -1 || (left + width) >= octopi.Length ? -1 : left + width;
            var bottomRight = right == -1 || (right + width) >= octopi.Length ? -1 : right + width;

            var neighbors = new int[]{
                index - width,
                left,
                right,
                bottom,
                left - width,
                right - width,
                bottomLeft,
                bottomRight
            };

            neighbors = neighbors.Where(neighborIndex => neighborIndex >= 0 && !flashed.Contains(neighborIndex)).ToArray();

            foreach (var neighborIndex in neighbors)
            {
                if (!flashed.Contains(neighborIndex))
                    octopi[neighborIndex] += (char)1;
            }

            foreach (var neighborIndex in neighbors)
            {
                if (octopi[neighborIndex] < ':')
                    continue;

                Flash(neighborIndex);

                octopi[neighborIndex] = '0';
            }
        };

        foreach (var step in Enumerable.Range(0, 100))
        {
            flashed = new List<int>();

            for (var index = 0; index < octopi.Length; index++)
            {
                //converting the array to ints is not necessary
                // as chars are pretty much ints already
                if (!flashed.Contains(index))
                    octopi[index] += (char)1;

                if (octopi[index] < ':')
                    continue;

                Flash(index);

                octopi[index] = '0';
            }
            //Draw();
        }

        Console.WriteLine($"Part 1: \x1b[93m{flashCount}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 1 Execution Time: {watch.ElapsedMilliseconds} ms");

        watch.Reset();
        watch.Start();

        //we've already completed 100 steps so keep stepping until
        // the flashed count is equal to the number of octopi
        var syncStep = 100;

        while (flashed.Count != octopi.Length)
        {
            flashed = new List<int>();

            for (var index = 0; index < octopi.Length; index++)
            {
                if (!flashed.Contains(index))
                    octopi[index] += (char)1;

                if (octopi[index] < ':')
                    continue;

                Flash(index);

                octopi[index] = '0';
            }

            syncStep += 1;
        }

        Console.WriteLine($"Part 1: \x1b[93m{syncStep}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 2 Execution Time: {watch.ElapsedMilliseconds} ms");
    }

    public static void Day12(string[] data)
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        var graph = new Graph();
        var paths = new List<List<string>>();

        foreach (var line in data)
        {
            var node = new GraphNode(line.Split('-')[0]);
            node.AddNeighbor(line.Split('-')[1]);
            graph.AddNode(node);

            node = new GraphNode(line.Split('-')[1]);
            node.AddNeighbor(line.Split('-')[0]);
            graph.AddNode(node);
        }

        Action<GraphNode, List<string>, bool> Visit = (node, currentPath, visitOneSmallTwice) => { };

        Visit = (node, currentPath, visitOneSmallTwice) =>
        {
            var iterationPath = new List<string>(currentPath);
            iterationPath.Add(node.Identifier);

            if (node.Identifier == "end")
            {
                paths.Add(iterationPath);
                return;
            }

            foreach (var neighbor in node.Neighbors)
            {
                if (neighbor[0] >= 97 && !iterationPath.Contains(neighbor)
                    || (neighbor[0] >= 97 && visitOneSmallTwice && iterationPath.GroupBy(path => path).Where(path => path.Key[0] >= 97).All(group => group.Count() < 2) && neighbor != "start"))
                {
                    Visit(graph.Nodes.Single(graphNode => graphNode.Identifier == neighbor), iterationPath, visitOneSmallTwice);
                }
                else if (neighbor[0] < 97)
                {
                    Visit(graph.Nodes.Single(graphNode => graphNode.Identifier == neighbor), iterationPath, visitOneSmallTwice);
                }
            }

            return;
        };

        Visit(graph.Nodes.Single(node => node.Identifier == "start"), new List<string>(), false);
        Console.WriteLine($"Part 1: \x1b[93m{paths.Count()}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 1 Execution Time: {watch.ElapsedMilliseconds} ms");

        watch.Reset();
        watch.Start();

        paths = new List<List<string>>();
        Visit(graph.Nodes.Single(node => node.Identifier == "start"), new List<string>(), true);

        Console.WriteLine($"Part 2: \x1b[93m{paths.Count()}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 2 Execution Time: {watch.ElapsedMilliseconds} ms");
    }

    public static void Day13(string[] data)
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        var dots = new HashSet<int[]>();
        var folds = new List<string>();

        foreach (var line in data)
        {
            if (line.IndexOf(',') > -1)
            {
                dots.Add(new int[] { int.Parse(line.Split(',')[0]), int.Parse(line.Split(',')[1]) });
            }
            else
            {
                folds.Add(line.Split(' ')[2]);
            }
        }

        var width = dots.Max(set => set[0]) + 1;
        var height = dots.Max(set => set[1]) + 1;

        var dotArray = Enumerable.Repeat('\x2591', width * height).ToArray();

        foreach (var dot in dots)
        {
            dotArray[dot[0] + dot[1] * width] = '\x2588';
        }

        foreach (var fold in folds.Take(1))
        {
            var split = int.Parse(fold.Split('=')[1]);

            if (fold.Split('=')[0] == "y")
            {
                foreach (var y in Enumerable.Range(split, height - split))
                {
                    foreach (var x in Enumerable.Range(0, width))
                    {
                        var newY = Math.Abs((y - split) - split);

                        if (dotArray[x + newY * width] != '\x2588' && dotArray[x + y * width] == '\x2588')
                        {
                            dotArray[x + newY * width] = '\x2588';
                        }

                        dotArray[x + y * width] = ' ';
                    }
                }
            }

            if (fold.Split('=')[0] == "x")
            {
                foreach (var y in Enumerable.Range(0, height))
                {
                    foreach (var x in Enumerable.Range(split, width - split))
                    {
                        var newX = Math.Abs((x - split) - split);

                        if (dotArray[newX + y * width] != '\x2588' && dotArray[x + y * width] == '\x2588')
                        {
                            dotArray[newX + y * width] = '\x2588';
                        }

                        dotArray[x + y * width] = ' ';
                    }
                }
            }
        }

        Console.WriteLine($"Part 1: \x1b[93m{dotArray.Count(dot => dot == '\x2588')}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 1 Execution Time: {watch.ElapsedMilliseconds} ms");

        watch.Reset();
        watch.Start();

        foreach (var fold in folds.Skip(1))
        {
            var split = int.Parse(fold.Split('=')[1]);

            if (fold.Split('=')[0] == "y")
            {
                foreach (var y in Enumerable.Range(split, height - split))
                {
                    foreach (var x in Enumerable.Range(0, width))
                    {
                        var newY = Math.Abs((y - split) - split);

                        if (dotArray[x + newY * width] != '\x2588' && dotArray[x + y * width] == '\x2588')
                        {
                            dotArray[x + newY * width] = '\x2588';
                        }

                        dotArray[x + y * width] = ' ';
                    }
                }
            }

            if (fold.Split('=')[0] == "x")
            {
                foreach (var y in Enumerable.Range(0, height))
                {
                    foreach (var x in Enumerable.Range(split, width - split))
                    {
                        var newX = Math.Abs((x - split) - split);

                        if (dotArray[newX + y * width] != '\x2588' && dotArray[x + y * width] == '\x2588')
                        {
                            dotArray[newX + y * width] = '\x2588';
                        }

                        dotArray[x + y * width] = ' ';
                    }
                }
            }
        }

        Console.WriteLine($"Part 2: \x1b[93m");
        foreach (var y in Enumerable.Range(0, height))
        {
            foreach (var x in Enumerable.Range(0, width))
            {
                if (dotArray[x + y * width] != ' ')
                    Console.Write(dotArray[x + y * width]);
            }

            if (dotArray[0 + y * width] != ' ')
                Console.Write(Environment.NewLine);
        }
        Console.WriteLine("\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 2 Execution Time: {watch.ElapsedMilliseconds} ms");
    }

    public static void Day14(string[] data)
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        var polymerTemplate = data[0];
        var insertionRules = new Dictionary<string, string>();
        var pairCounts = new Dictionary<string, long>();
        var elementCounts = new Dictionary<char, long>();

        foreach (var line in data.Skip(1))
        {
            insertionRules.Add(line.Split(" -> ")[0], line.Split(" -> ")[1]);
            pairCounts.Add(line.Split(" -> ")[0], 0);
            elementCounts.TryAdd(line.Split(" -> ")[0][0], 0);
            elementCounts.TryAdd(line.Split(" -> ")[0][1], 0);
        }

        polymerTemplate.GroupBy(character => character).ToList().ForEach(group => elementCounts[group.Key] = group.Count());
        polymerTemplate.Zip(polymerTemplate.Skip(1), (a, b) => string.Join("", new char[] { a, b })).ToList().ForEach(pair => pairCounts[pair] += 1);

        foreach (var step in Enumerable.Range(0, 10))
        {
            var newPairCounts = new Dictionary<string, long>(pairCounts);
            newPairCounts = newPairCounts.ToDictionary(p => p.Key, p => (long)0);

            foreach (var pair in pairCounts.Where(kvp => kvp.Value > 0))
            {
                var insert = insertionRules[pair.Key];
                elementCounts[insert[0]] += (1 * pair.Value);
                newPairCounts[pair.Key[0] + insert] += pairCounts[pair.Key];
                newPairCounts[insert + pair.Key[1]] += pairCounts[pair.Key];
            }

            pairCounts = newPairCounts;
        }

        Console.WriteLine($"Part 1: \x1b[93m{elementCounts.Max(kvp => kvp.Value) - elementCounts.Min(kvp => kvp.Value)}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 1 Execution Time: {watch.ElapsedMilliseconds} ms");

        watch.Reset();
        watch.Start();

        foreach (var step in Enumerable.Range(0, 30))
        {
            var newPairCounts = new Dictionary<string, long>(pairCounts);
            newPairCounts = newPairCounts.ToDictionary(p => p.Key, p => (long)0);

            foreach (var pair in pairCounts.Where(kvp => kvp.Value > 0))
            {
                var insert = insertionRules[pair.Key];
                elementCounts[insert[0]] += (1 * pair.Value);
                newPairCounts[pair.Key[0] + insert] += pairCounts[pair.Key];
                newPairCounts[insert + pair.Key[1]] += pairCounts[pair.Key];
            }

            pairCounts = newPairCounts;
        }

        Console.WriteLine($"Part 2: \x1b[93m{elementCounts.Max(kvp => kvp.Value) - elementCounts.Min(kvp => kvp.Value)}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 2 Execution Time: {watch.ElapsedMilliseconds} ms");
    }

    public static void Day15(string[] data)
    {
        //Speed could be greatly improved using a better pathing algorithm.
        // Part one completes in 315 ms and part two take nearly 3 minutes
        // Not entirely sure where the performance can be improved but maybe
        // I'll figure it out some day.

        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        var formattedData = data.SelectMany(line => Array.ConvertAll(line.ToArray(), character => int.Parse(character.ToString()))).ToList();
        var graphSize = data[0].Length * data.Length;
        var graphWidth = data[0].Length;

        var graph = Enumerable.Range(0, graphSize).Select(i => (Enumerable.Repeat(new Dictionary<int, int>(), 4).ToArray())).ToArray();

        foreach (var vertex in formattedData.WithIndex())
        {
            var left = vertex.Index % graphWidth == 0 ? -1 : vertex.Index - 1;
            var right = (vertex.Index + 1) % graphWidth == 0 ? -1 : vertex.Index + 1;
            var bottom = (vertex.Index + graphWidth) >= graphSize ? -1 : vertex.Index + graphWidth;
            var top = (vertex.Index - graphWidth);

            if (top >= 0)
                graph[vertex.Index][0] = new Dictionary<int, int> { { top, formattedData.ElementAt(top) } };

            if (left >= 0)
                graph[vertex.Index][1] = new Dictionary<int, int> { { left, formattedData.ElementAt(left) } };

            if (right >= 0)
                graph[vertex.Index][2] = new Dictionary<int, int> { { right, formattedData.ElementAt(right) } };

            if (bottom >= 0)
                graph[vertex.Index][3] = new Dictionary<int, int> { { bottom, formattedData.ElementAt(bottom) } };
        }

        var answer = Helpers.DijkstraAlgo(graph);

        Console.WriteLine($"Part 1: \x1b[93m{answer.Last()}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 1 Execution Time: {watch.ElapsedMilliseconds} ms");

        watch.Reset();
        watch.Start();

        var newData = new List<int>(formattedData);
        var newGraphWidth = data[0].Length * 5;

        //create first grid row
        foreach (var row in Enumerable.Range(0, data.Length))
        {
            //generate second grid
            var additionalData = new List<int>(formattedData.Skip(row * graphWidth)
                                          .Take(graphWidth)
                                          .Select(index =>
                                          {
                                              if (index == 9)
                                                  return 1;

                                              return index + 1;
                                          }));

            foreach (var iteration in Enumerable.Range(0, 3))
            {
                additionalData.AddRange(additionalData.Skip(iteration * graphWidth)
                                          .Take(graphWidth)
                                          .Select(index =>
                                          {
                                              if (index == 9)
                                                  return 1;

                                              return index + 1;
                                          }));
            }

            var insertPoint = (row * (graphWidth + additionalData.Count())) + graphWidth;

            newData.InsertRange(insertPoint, additionalData);
        }

        //generate the rest of the rows
        foreach (var row in Enumerable.Range(0, data.Length * 4))
        {
            //generate second grid
            var additionalData = new List<int>(newData.Skip(row * newGraphWidth)
                                          .Take(newGraphWidth)
                                          .Select(index =>
                                          {
                                              if (index == 9)
                                                  return 1;

                                              return index + 1;
                                          }));

            newData.AddRange(additionalData);
        }

        var newGraphSize = newData.Count();

        graph = Enumerable.Range(0, newGraphSize).Select(i => (Enumerable.Repeat(new Dictionary<int, int>(), 4).ToArray())).ToArray();

        foreach (var vertex in newData.WithIndex())
        {
            var left = vertex.Index % newGraphWidth == 0 ? -1 : vertex.Index - 1;
            var right = (vertex.Index + 1) % newGraphWidth == 0 ? -1 : vertex.Index + 1;
            var bottom = (vertex.Index + newGraphWidth) >= newGraphSize ? -1 : vertex.Index + newGraphWidth;
            var top = (vertex.Index - newGraphWidth);

            if (top >= 0)
                graph[vertex.Index][0] = new Dictionary<int, int> { { top, newData.ElementAt(top) } };

            if (left >= 0)
                graph[vertex.Index][1] = new Dictionary<int, int> { { left, newData.ElementAt(left) } };

            if (right >= 0)
                graph[vertex.Index][2] = new Dictionary<int, int> { { right, newData.ElementAt(right) } };

            if (bottom >= 0)
                graph[vertex.Index][3] = new Dictionary<int, int> { { bottom, newData.ElementAt(bottom) } };
        }

        answer = Helpers.DijkstraAlgo(graph);

        Console.WriteLine($"Part 2: \x1b[93m{answer.Last()}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 2 Execution Time: {watch.ElapsedMilliseconds} ms");
    }

    public static void Day16(string[] data)
    {
        var packet = data.First();

        var map = new Dictionary<char, string> {
                {'0' , "0000"},
                {'1' , "0001"},
                {'2' , "0010"},
                {'3' , "0011"},
                {'4' , "0100"},
                {'5' , "0101"},
                {'6' , "0110"},
                {'7' , "0111"},
                {'8' , "1000"},
                {'9' , "1001"},
                {'A' , "1010"},
                {'B' , "1011"},
                {'C' , "1100"},
                {'D' , "1101"},
                {'E' , "1110"},
                {'F' , "1111"}
            };

        var binaryPacket = string.Join("", packet.Select(character => map[character]));

        var decoder = new Packet(binaryPacket);

        Console.WriteLine($"Part 1: \x1b[93m{decoder.GetVersionSum()}\x1b[0m");
        Console.WriteLine($"Part 2: \x1b[93m{decoder.Value}\x1b[0m");
    }

    public static void Day17(string[] data)
    {
        var targetArea = data.First();
        //var targetArea = "target area: x=20..30, y=-10..-5";

        var targetAreaX = Array.ConvertAll(targetArea[(targetArea.IndexOf("x=") + 2)..targetArea.IndexOf(",")].Split(".."), int.Parse);
        var targetAreaY = Array.ConvertAll(targetArea.Substring(targetArea.IndexOf("y=") + 2).Split(".."), int.Parse);

        var maxY = 0;
        var minX = 0;

        var maxHeight = 0;

        //took me a minute to realize the velocities were changing like traingle numbers
        // but once I did the calculations became easy

        for (int i = 1; i <= targetAreaX[1]; i++)
        {
            int triangleNumber = (i * (i + 1)) / 2;

            if (triangleNumber >= targetAreaX[0] && triangleNumber <= targetAreaX[1])
            {
                minX = i;
                break;
            }

            if (triangleNumber > targetAreaX[1])
                break;
        }

        for (int i = 1; i <= -targetAreaY[0]; i++)
        {
            int triangleNumber = (i * (i + 1)) / 2;

            for (int j = 1; ; j++)
            {
                int negativeTriangleNumber = ((j * (j + 1)) / 2) * -1;

                //what goes up must come down (and land in the target area)
                int difference = negativeTriangleNumber + triangleNumber;

                if (difference >= targetAreaY[0] && difference <= targetAreaY[1])
                {
                    if (triangleNumber >= maxY)
                    {
                        maxY = i;
                        maxHeight = triangleNumber;
                    }
                    else
                        break;
                }

                if (difference < targetAreaY[0])
                    break;
            }
        }

        Console.WriteLine(maxHeight);

        var possibleVelocities = 0;

        //I wonder if there's a more clever way to do this than running simulations for each combination of
        // velocities
        foreach (var x in Enumerable.Range(minX, targetAreaX[1] - minX + 1))
        {
            foreach (var y in Enumerable.Range(targetAreaY[0], maxY - targetAreaY[0] + 1))
            {
                var iterationX = x;
                var iterationY = y;
                var probePos = new int[] { 0, 0 };

                while (true)
                {
                    probePos[0] += iterationX;
                    probePos[1] += iterationY;

                    if (probePos[0] > targetAreaX[1] || probePos[1] < targetAreaY[0])
                        break;

                    if (iterationX != 0)
                        iterationX += iterationX > 0 ? -1 : 1;

                    iterationY -= 1;

                    if ((probePos[0] >= targetAreaX[0] && probePos[0] <= targetAreaX[1])
                     && (probePos[1] >= targetAreaY[0] && probePos[1] <= targetAreaY[1]))
                    {
                        possibleVelocities++;
                        break;
                    }
                }
            }
        }

        Console.WriteLine(possibleVelocities);
    }
    #endregion

    public static void Day18(string[] data)
    {
        var homework = new List<Pair>();

        foreach (var line in data)
        {
            var stack = new Stack<Pair>();
            Pair snailfishNumber = new Pair();

            foreach (var character in line)
            {
                switch (character)
                {
                    case '[':
                        snailfishNumber = new Pair();
                        stack.Push(snailfishNumber);
                        break;
                    case ']':
                        snailfishNumber = stack.Pop();

                        if(stack.Count > 0)
                        {
                            snailfishNumber.Parent = stack.Peek();

                            if (snailfishNumber.Parent.Left is null)
                                snailfishNumber.Parent.Left = snailfishNumber;
                            else
                                snailfishNumber.Parent.Right = snailfishNumber;

                            snailfishNumber = snailfishNumber.Parent;
                        }
                        break;
                    case ',':
                        continue;
                    default:
                        if(snailfishNumber.Left is null)
                            snailfishNumber.Left = int.Parse(character.ToString());
                        else
                            snailfishNumber.Right = int.Parse(character.ToString());
                        break;
                }
            }

            homework.Add(snailfishNumber);
        }

        homework.ForEach(line => Console.WriteLine(line.ToString()));

        // I think adding trees together will be setting the first one as the left value and the second as the right
        // of a new Pair object. It's 4 am on a shcool night and I should have been in bed a long time ago

        //probably gonna use breadth first search for finding pairs in the tree to explode and split to




    }
}
