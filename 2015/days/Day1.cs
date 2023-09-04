namespace Advent2015;

public static class Day1
{
    public static void Run(string[] data)
    {
        var answer = data[0].Where(character => character == '(').Count() - data[0].Where(character => character == ')').Count();

        Console.WriteLine($"Part 1: {answer}");

        var floor = 0;
        answer = 0;

        foreach (var character in data[0])
        {
            if (floor == -1)
                break;

            answer += 1;

            floor += character == '(' ? 1 : -1;
        }

        Console.WriteLine($"Part 2: {answer}");
    }
}