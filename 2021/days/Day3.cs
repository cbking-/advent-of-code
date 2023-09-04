namespace Advent2021;

public static class Day3
{
    public static void Run(string[] data)
    {
        var gammaRate = new StringBuilder();
        var epsilonRate = new StringBuilder();

        foreach (var index in Enumerable.Range(0, data[0].Length))
        {
            var groups = data.GroupBy(line => line.ElementAt(index));
            gammaRate.Append(groups.OrderBy(group => group.Count()).First().Key);
            epsilonRate.Append(groups.OrderByDescending(group => group.Count()).First().Key);
        }

        Console.WriteLine($"Part 1: {Convert.ToInt32(gammaRate.ToString(), 2) * Convert.ToInt32(epsilonRate.ToString(), 2)}");

        var diagnosticData = new List<String>(data);
        var oxyGenRate = new List<string>(data);
        var co2ScrubRate = new List<string>(data);

        foreach (var index in Enumerable.Range(0, data[0].Length))
        {
            var groups = diagnosticData.GroupBy(line => line.ElementAt(index));
            oxyGenRate = oxyGenRate.Intersect(groups.OrderByDescending(group => group.Count())
                                                    .ThenByDescending(group => group.Key)
                                                    .First()
                                                    .Select(group => group))
                                                .ToList();
            diagnosticData = diagnosticData.Intersect(oxyGenRate).ToList();
        }

        diagnosticData = new List<String>(data);

        foreach (var index in Enumerable.Range(0, data[0].Length))
        {
            var groups = diagnosticData.GroupBy(line => line.ElementAt(index));
            co2ScrubRate = co2ScrubRate.Intersect(groups.OrderBy(group => group.Count())
                                                        .ThenBy(group => group.Key)
                                                        .First()
                                                        .Select(group => group))
                                                    .ToList();
            diagnosticData = diagnosticData.Intersect(co2ScrubRate).ToList();
        }

        Console.WriteLine($"Part 2: {Convert.ToInt32(oxyGenRate.Single(), 2) * Convert.ToInt32(co2ScrubRate.Single(), 2)}");
    }

}