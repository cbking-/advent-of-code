namespace Advent2021;

public static class Day10
{
    public static void Run(string[] data)
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        var chunckDefs = new Dictionary<char, char>(){
            {')' , '('},
            {']' , '['},
            {'}' , '{'},
            {'>' , '<'}
        };

        var pointsDef = new Dictionary<char, int>(){
            {')' , 3},
            {']' , 57},
            {'}' , 1197},
            {'>' , 25137}
        };

        var pointsDefTwo = new Dictionary<char, int>(){
            {')' , 1},
            {']' , 2},
            {'}' , 3},
            {'>' , 4}
        };

        var partOne = 0;
        var partTwoScores = new List<long>();

        foreach (var line in data)
        {
            var chunckStack = new Stack<char>();
            var corrupt = false;
            foreach (var character in line)
            {
                if (chunckStack.Count == 0 || chunckDefs.ContainsValue(character))
                {
                    chunckStack.Push(character);
                }
                else if (chunckDefs[character] == chunckStack.Peek())
                {
                    chunckStack.Pop();
                }
                else
                {
                    var key = chunckDefs.Where(kvp => kvp.Value == chunckStack.Peek()).Single().Key;
                    partOne += pointsDef[character];
                    //Console.WriteLine($"Expected {key}, but found {character} instead");
                    corrupt = true;
                }

                if (corrupt)
                    break;
            }

            if (!corrupt)
            {
                var finishers = chunckStack.Select(chunkStart => chunckDefs.Where(kvp => kvp.Value == chunkStart).Single().Key);
                var partTwo = finishers.Aggregate<char, long>(0, (acc, finisher) => (acc * 5) + pointsDefTwo[finisher]);
                partTwoScores.Add(partTwo);
            }
        }

        Console.WriteLine($"Part 1: \x1b[93m{partOne}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 1 Execution Time: {watch.ElapsedMilliseconds} ms");

        watch.Reset();
        watch.Start();
        Console.WriteLine($"Part 2: \x1b[93m{partTwoScores.OrderBy(score => score).Take((partTwoScores.Count() / 2) + 1).Last()}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 2 Execution Time: {watch.ElapsedMilliseconds} ms");
    }

}