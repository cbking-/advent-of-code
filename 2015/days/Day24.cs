namespace Advent2015;

public static class Day24
{
    public static void Run(string[] data)
    {
        var packages = Array.ConvertAll(data, long.Parse);
        var groupSum = packages.Sum() / 3;

        var possibleGroups = new List<IReadOnlyCollection<long>>();

        foreach (var size in Enumerable.Range(1, (int)Math.Ceiling((double)packages.Length / 2)))
        {
            var combos = new Combinations<long>(packages, size);
            possibleGroups.AddRange(combos.Where(combo => combo.Sum() == groupSum));
        }

        var quantumEntaglement = possibleGroups.GroupBy(group => group.Count)
                                           .OrderBy(groups => groups.Key)
                                           .First() //smallest groups possible
                                           .Select(group => group.Aggregate((total, package) => total *= package))
                                           .OrderBy(qe => qe)
                                           .First();

        Console.WriteLine($"Part 1: {quantumEntaglement}");

        groupSum = packages.Sum() / 4;

        possibleGroups = new List<IReadOnlyCollection<long>>();

        foreach (var size in Enumerable.Range(1, (int)Math.Ceiling((double)packages.Length / 2)))
        {
            var combos = new Combinations<long>(packages, size);
            possibleGroups.AddRange(combos.Where(combo => combo.Sum() == groupSum));
        }

        quantumEntaglement = possibleGroups.GroupBy(group => group.Count)
                                           .OrderBy(groups => groups.Key)
                                           .First() //smallest groups possible
                                           .Select(group => group.Aggregate((total, package) => total *= package))
                                           .OrderBy(qe => qe)
                                           .First();

        Console.WriteLine($"Part 2: {quantumEntaglement}");
    }

}