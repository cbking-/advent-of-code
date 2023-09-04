namespace Advent2022;

public static class Day10
{
    public static void Run(string[] data)
    {
        int[] cycleChecks = { 20, 60, 100, 140, 180, 220 };
        int[] signalStrenths = { 0, 0, 0, 0, 0, 0 };

        var cycleNumber = 0;
        var x = 1;
        var pixelIndex = 0;

        Action IncrementCycle = () =>
        {
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

            if (cycleNumber % 40 == 0)
            {
                pixelIndex = 0;
                Console.Write(Environment.NewLine);
            }

            if (cycleChecks.Contains(cycleNumber))
            {
                var signalIndex = Array.IndexOf(cycleChecks, cycleNumber);
                signalStrenths[signalIndex] = cycleNumber * x;
            }

        };

        foreach (var line in data)
        {
            var instruction = line.Split(' ');
            var command = instruction[0];
            int.TryParse(instruction.ElementAtOrDefault(1), out int argument);

            IncrementCycle();

            if (command == "addx")
            {
                IncrementCycle();
                x += argument;
            }
        }

        Console.WriteLine(signalStrenths.Sum());
    }

}