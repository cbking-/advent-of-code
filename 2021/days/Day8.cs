namespace Advent2021;

public static class Day8
{
    public static void Run(string[] data)
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

}