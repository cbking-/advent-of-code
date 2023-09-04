namespace Advent2021;

public static class Day5
{
    public static void Run(string[] data)
    {
        //There are some tuple swaps you can do
        // when comparing coordinates so the for loops are more condensed
        // E.g. for diagonals
        //   if(xStart > xEnd)
        //      ((xStart, yStart), (xEnd, yEnd)) = ((xEnd, yEnd), (xStart, yStart))
        //
        // Could also use a HashSet instead of a List and
        // increment some count when HashSet.Add returns false

        var straightVents = new List<(int, int)>();
        var diagVents = new List<(int, int)>();

        foreach (var set in data)
        {
            var start = set.Split(" -> ")[0];
            var end = set.Split(" -> ")[1];

            var xStart = int.Parse(start.Split(',')[0]);
            var yStart = int.Parse(start.Split(',')[1]);
            var xEnd = int.Parse(end.Split(',')[0]);
            var yEnd = int.Parse(end.Split(',')[1]);

            var steps = Math.Abs(xStart - xEnd) + 1;

            if (xStart == xEnd)
            {
                foreach (var yCoord in Enumerable.Range(Math.Min(yStart, yEnd), Math.Abs(yStart - yEnd) + 1))
                {
                    straightVents.Add((xStart, yCoord));
                }
            }
            else if (yStart == yEnd)
            {
                foreach (var xCoord in Enumerable.Range(Math.Min(xStart, xEnd), Math.Abs(xStart - xEnd) + 1))
                {
                    straightVents.Add((xCoord, yStart));
                }
            }
            else if (xStart < xEnd && yStart < yEnd)
            {
                //starting top left
                foreach (var step in Enumerable.Range(0, steps))
                {
                    diagVents.Add((xStart + step, yStart + step));
                }
            }
            else if (xStart > xEnd && yStart < yEnd)
            {

                //starting top right
                foreach (var step in Enumerable.Range(0, steps))
                {
                    diagVents.Add((xStart - step, yStart + step));
                }
            }
            else if (xStart < xEnd && yStart > yEnd)
            {
                //starting bottom left
                foreach (var step in Enumerable.Range(0, steps))
                {
                    diagVents.Add((xStart + step, yStart - step));
                }
            }
            else if (xStart > xEnd && yStart > yEnd)
            {
                //starting bottom right
                foreach (var step in Enumerable.Range(0, steps))
                {
                    diagVents.Add((xStart - step, yStart - step));
                }
            }
        }

        var partOne = straightVents.GroupBy(vent => vent).Where(vent => vent.Count() > 1).Count();

        Console.WriteLine($"Part 1: \x1b[93m{partOne}\x1b[0m");

        var partTwo = straightVents.Concat(diagVents).GroupBy(vent => vent).Where(vent => vent.Count() > 1).Count();

        Console.WriteLine($"Part 2: \x1b[93m{partTwo}\x1b[0m");
    }

}