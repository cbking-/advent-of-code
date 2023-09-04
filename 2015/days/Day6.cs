namespace Advent2015;

public static class Day6
{
    public static void Run(string[] data)
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
}