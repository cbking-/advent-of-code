namespace Advent2022;

public static class Day6
{
    public static void Run(string[] data)
    {
        var line = data.First();

        for (var n = 0; n < line.Length; n++)
        {
            if (line.Skip(n).Take(4).Distinct().Count() == 4)
            {
                Console.WriteLine(n + 4);
                break;
            }
        }

        for (var n = 0; n < line.Length; n++)
        {
            if (line.Skip(n).Take(14).Distinct().Count() == 14)
            {
                Console.WriteLine(n + 14);
                break;
            }
        }
    }

}