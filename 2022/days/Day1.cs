namespace Advent2022;

public static class Day1
{
    public static void Run(string[] data)
    {
        var elves = data.Aggregate(new List<List<int>> { new() }, (acc, line) =>
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                acc.Add(new List<int>());
            }
            else
            {
                acc.Last().Add(int.Parse(line));
            }

            return acc;
        });

        Console.WriteLine(elves.Select(elf => elf.Sum()).Max());
        Console.WriteLine(elves.Select(elf => elf.Sum()).OrderByDescending(total => total).Take(3).Sum());
    }

}