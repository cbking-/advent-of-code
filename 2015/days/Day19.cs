namespace Advent2015;

public static class Day19
{
    public static void Run(string[] data)
    {
        //key will be the replacement and value is the replaced
        var maps = new Dictionary<string, string>();
        var molecule = "";
        var replacementMolecules = new HashSet<string>();

        foreach (var line in data)
        {
            //last line is the medicine molecule
            if (line == data.Last())
            {
                molecule = line;
            }
            else
            {
                maps.Add(line.Split(" => ")[1], line.Split(" => ")[0]);
            }
        }

        var mapGroups = maps.GroupBy(kvp => kvp.Value);

        foreach (var mapGroup in mapGroups)
        {
            var moleculeToReplace = mapGroup.Key;

            foreach (var replacement in mapGroup)
            {
                var match = Regex.Match(molecule, moleculeToReplace);

                while (match.Success)
                {
                    var capture = match.Captures.First();
                    var newMolecule = molecule.Remove(capture.Index, capture.Length).Insert(capture.Index, replacement.Key);
                    replacementMolecules.Add(newMolecule);
                    match = match.NextMatch();
                }
            }
        }

        Console.WriteLine($"Part 1: {replacementMolecules.Count}");

        //seems like you need to know some logic and/or more advanced algorithms
        // shamelessly stealing logic from top comment of reddit solution thread
        // https://www.reddit.com/r/adventofcode/comments/3xflz8/comment/cy4etju/?utm_source=share&utm_medium=web2x&context=3

        //I think some before this one haven't *required* code but this one seemed (to me) to require some understanding
        // of the world that I don't have.
        var elements = Regex.Matches(molecule, "[A-Z][a-z]?").Count;
        var radon = Regex.Matches(molecule, "Rn").Count;
        var argon = Regex.Matches(molecule, "Ar").Count;
        var yttrium = Regex.Matches(molecule, "Y").Count;

        var answer = elements - (radon + argon) - (yttrium * 2) - 1;
        Console.WriteLine($"Part 2: {answer}");
    }
}