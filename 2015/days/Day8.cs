namespace Advent2015;

public static class Day8
{
    public static void Run(string[] data)
    {
        var characterSum = data.Sum(line => line.Length);

        var memorySum = data.Sum(line =>
        {
            line = line[1..^1];
            line = Regex.Replace(line, @"\\\\", @"A");
            line = Regex.Replace(line, @"\\""", @"A");
            line = Regex.Replace(line, @"\\x[0-9a-fA-F]{2}", @"A");
            return line.Length;
        });

        Console.WriteLine($"Part 1: {characterSum - memorySum}");

        var encodedSum = data.Sum(line =>
        {
            //some lines end with \\" which will mess up replacment later
            line = line[1..^1];
            line = Regex.Replace(line, @"\\", @"\\");
            line = Regex.Replace(line, @"\\""", @"\\""");

            return line.Length + 6; //4 being the encode quotes and 2 the surrounding quotes
        });

        Console.WriteLine($"Part 2: {encodedSum - characterSum}");
    }

}