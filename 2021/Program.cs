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

    #endregion

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
}
