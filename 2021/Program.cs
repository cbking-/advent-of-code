﻿using System.Diagnostics;
using System.Reflection;
using System.Text;
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
                if(partOne == 0)
                    partOne = boards.Where(board => board.HasBingo()).Single().Places.Where(place => !place.Marked).Sum(place => place.Number) * draw;

                if(boards.Count() == 1)
                    partTwo = boards.Single().Places.Where(place => !place.Marked).Sum(place => place.Number) * draw;

                boards.RemoveAll(board => board.HasBingo());
            }
        }

        Console.WriteLine($"Part 1: \x1b[93m{partOne}\x1b[0m");
        Console.WriteLine($"Part 2: \x1b[93m{partTwo}\x1b[0m");
    }
}
