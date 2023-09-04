namespace Advent2015;

public static class Day3
{
    public static void Run(string[] data)
    {
        var houses = new HashSet<int[]>(new IntArrayKeyComparer());
        var x = 0;
        var y = 0;
        houses.Add(new int[] { x, y });

        foreach (var character in data[0])
        {
            if (character == '^')
                y -= 1;

            if (character == 'v')
                y += 1;

            if (character == '<')
                x -= 1;

            if (character == '>')
                x += 1;

            houses.Add(new int[] { x, y });
        }

        Console.WriteLine($"Part 1: {houses.Count()}");

        houses = new HashSet<int[]>(new IntArrayKeyComparer());
        x = 0;
        y = 0;
        houses.Add(new int[] { x, y });

        foreach (var character in data[0].Where((character, index) => index % 2 == 0))
        {
            if (character == '^')
                y -= 1;

            if (character == 'v')
                y += 1;

            if (character == '<')
                x -= 1;

            if (character == '>')
                x += 1;

            houses.Add(new int[] { x, y });
        }

        x = 0;
        y = 0;

        foreach (var character in data[0].Where((character, index) => index % 2 != 0))
        {
            if (character == '^')
                y -= 1;

            if (character == 'v')
                y += 1;

            if (character == '<')
                x -= 1;

            if (character == '>')
                x += 1;

            houses.Add(new int[] { x, y });
        }

        Console.WriteLine($"Part 2: {houses.Count()}");
    }
}