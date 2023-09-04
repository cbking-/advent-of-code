namespace Advent2021;

public static class Day14
{
    public static void Run(string[] data)
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        var polymerTemplate = data[0];
        var insertionRules = new Dictionary<string, string>();
        var pairCounts = new Dictionary<string, long>();
        var elementCounts = new Dictionary<char, long>();

        foreach (var line in data.Skip(1))
        {
            insertionRules.Add(line.Split(" -> ")[0], line.Split(" -> ")[1]);
            pairCounts.Add(line.Split(" -> ")[0], 0);
            elementCounts.TryAdd(line.Split(" -> ")[0][0], 0);
            elementCounts.TryAdd(line.Split(" -> ")[0][1], 0);
        }

        polymerTemplate.GroupBy(character => character).ToList().ForEach(group => elementCounts[group.Key] = group.Count());
        polymerTemplate.Zip(polymerTemplate.Skip(1), (a, b) => string.Join("", new char[] { a, b })).ToList().ForEach(pair => pairCounts[pair] += 1);

        foreach (var step in Enumerable.Range(0, 10))
        {
            var newPairCounts = new Dictionary<string, long>(pairCounts);
            newPairCounts = newPairCounts.ToDictionary(p => p.Key, p => (long)0);

            foreach (var pair in pairCounts.Where(kvp => kvp.Value > 0))
            {
                var insert = insertionRules[pair.Key];
                elementCounts[insert[0]] += (1 * pair.Value);
                newPairCounts[pair.Key[0] + insert] += pairCounts[pair.Key];
                newPairCounts[insert + pair.Key[1]] += pairCounts[pair.Key];
            }

            pairCounts = newPairCounts;
        }

        Console.WriteLine($"Part 1: \x1b[93m{elementCounts.Max(kvp => kvp.Value) - elementCounts.Min(kvp => kvp.Value)}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 1 Execution Time: {watch.ElapsedMilliseconds} ms");

        watch.Reset();
        watch.Start();

        foreach (var step in Enumerable.Range(0, 30))
        {
            var newPairCounts = new Dictionary<string, long>(pairCounts);
            newPairCounts = newPairCounts.ToDictionary(p => p.Key, p => (long)0);

            foreach (var pair in pairCounts.Where(kvp => kvp.Value > 0))
            {
                var insert = insertionRules[pair.Key];
                elementCounts[insert[0]] += (1 * pair.Value);
                newPairCounts[pair.Key[0] + insert] += pairCounts[pair.Key];
                newPairCounts[insert + pair.Key[1]] += pairCounts[pair.Key];
            }

            pairCounts = newPairCounts;
        }

        Console.WriteLine($"Part 2: \x1b[93m{elementCounts.Max(kvp => kvp.Value) - elementCounts.Min(kvp => kvp.Value)}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 2 Execution Time: {watch.ElapsedMilliseconds} ms");
    }

}