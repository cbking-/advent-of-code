namespace Advent2015;

public class Instruction
{
    public string LeftOperand { get; set; } = string.Empty;
    public string RightOperand { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public string DestinationWire { get; set; } = string.Empty;
}

public static class Day7
{
    public static void Run(string[] data)
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

}