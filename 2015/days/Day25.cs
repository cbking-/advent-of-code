namespace Advent2015;

public static class Day25
{
    public static void Run(string[] data)
    {
        var targets = Regex.Matches(data[0], @"\d+");

        var targetRow = int.Parse(targets.First().Groups[0].Value);
        var targetCol = int.Parse(targets.Skip(1).First().Groups[0].Value);

        long code = 20151125;

        var row = 1;
        var col = 1;

        while (true)
        {
            if (row == 1)
            {
                row = col + 1;
                col = 1;
            }
            else
            {
                row -= 1;
                col += 1;
            }

            code = (code * 252533) % 33554393;

            if (row == targetRow && col == targetCol)
            {
                break;
            }
        }

        Console.WriteLine(code);

    }

}