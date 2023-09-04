namespace Advent2015;

public static class Day2
{
    public static void Run(string[] data)
    {
        var answer = data.Select(row => row.Split('x'))
                                           .Sum(dimension =>
                                           {
                                               var length = int.Parse(dimension[0]);
                                               var width = int.Parse(dimension[1]);
                                               var height = int.Parse(dimension[2]);

                                               var sideOne = length * width;
                                               var sideTwo = width * height;
                                               var sideThree = length * height;
                                               var smallSide = (new[] { sideOne, sideTwo, sideThree }).Min();

                                               return (2 * sideOne) + (2 * sideTwo) + (2 * sideThree) + smallSide;
                                           });

        Console.WriteLine($"Part 1: {answer}");

        answer = data.Select(row => row.Split('x'))
                                  .Sum(dimension =>
                                  {
                                      var length = int.Parse(dimension[0]);
                                      var width = int.Parse(dimension[1]);
                                      var height = int.Parse(dimension[2]);
                                      var smallestPerim = (new[] { length, width, height }).OrderBy(num => num).Take(2).Sum() * 2;

                                      var volume = length * width * height;

                                      return smallestPerim + volume;
                                  });

        Console.WriteLine($"Part 2: {answer}");
    }
}