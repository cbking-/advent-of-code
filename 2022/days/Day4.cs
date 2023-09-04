namespace Advent2022;

public static class Day4
{
    public static void Run(string[] data)
    {
        var overlapping = data.Aggregate(new int[] { 0, 0 }, (acc, line) =>
        {
            var elfOne = line.Split(',').First().Split('-').Select(Int32.Parse);
            var elfTwo = line.Split(',').Last().Split('-').Select(Int32.Parse);

            //completely overlapping
            if ((elfOne.First() <= elfTwo.First() && elfOne.Last() >= elfTwo.Last())
             || (elfTwo.First() <= elfOne.First() && elfTwo.Last() >= elfOne.Last()))
                acc[0] += 1;

            //partially overlapping (is the end of one in the other's range)
            if ((elfOne.Last() >= elfTwo.First() && elfOne.Last() <= elfTwo.Last())
             || (elfTwo.Last() >= elfOne.First() && elfTwo.Last() <= elfOne.Last()))
                acc[1] += 1;

            return acc;
        });

        Console.WriteLine(overlapping[0]);
        Console.WriteLine(overlapping[1]);
    }

}