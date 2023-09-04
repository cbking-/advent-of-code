namespace Advent2015;

public static class Day17
{
    public static void Run(string[] data)
    {
        var totalWays = 0;
        var minTotalWays = new ConcurrentDictionary<int, int>();

        for (var i = 1; i <= data.Count(); i++)
        {
            foreach (var combination in new Combinations<int>(data.ToList().ConvertAll(int.Parse), i, GenerateOption.WithoutRepetition))
            {
                if (combination.Sum() == 150)
                {
                    minTotalWays.AddOrUpdate(i, 1, (key, oldValue) => oldValue + 1);
                    totalWays += 1;
                }
            }
        }

        Console.WriteLine($"Part 1: {totalWays}");
        Console.WriteLine($"Part 2: {minTotalWays[minTotalWays.Min(kvp => kvp.Key)]}");
    }

}