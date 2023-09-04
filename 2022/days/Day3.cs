namespace Advent2022;

public static class Day3
{
    public static void Run(string[] data)
    {
        var prioritySum = data.Aggregate(0, (acc, line) =>
        {
            var compartmentOne = line.Take(line.Length / 2);
            var compartmentTwo = line.Skip(line.Length / 2);

            var commonItem = compartmentOne.Intersect(compartmentTwo);

            var priority = (int)(char)commonItem.Single();

            if (priority >= 97 && priority <= 122)
                priority -= 96;
            else
                priority -= 38;

            return acc + priority;
        });

        var badgeSum = data.Batch(3).Aggregate(0, (acc, group) =>
        {
            var priority = (int)group.Skip(1).Aggregate(group.First(), (previous, current) =>
                string.Concat(previous.Intersect(current))
            ).Single();

            if (priority >= 97 && priority <= 122)
                priority -= 96;
            else
                priority -= 38;

            return acc + priority;
        });

        Console.WriteLine(prioritySum);
        Console.WriteLine(badgeSum);
    }

}