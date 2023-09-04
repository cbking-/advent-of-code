namespace Advent2015;

public static class Day20
{
    public static void Run(string[] data)
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

}