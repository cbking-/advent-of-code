namespace Advent2021;

public static class Day17
{
    public static void Run(string[] data)
    {
        var targetArea = data.First();
        //var targetArea = "target area: x=20..30, y=-10..-5";

        var targetAreaX = Array.ConvertAll(targetArea[(targetArea.IndexOf("x=") + 2)..targetArea.IndexOf(",")].Split(".."), int.Parse);
        var targetAreaY = Array.ConvertAll(targetArea.Substring(targetArea.IndexOf("y=") + 2).Split(".."), int.Parse);

        var maxY = 0;
        var minX = 0;

        var maxHeight = 0;

        //took me a minute to realize the velocities were changing like traingle numbers
        // but once I did the calculations became easy

        for (int i = 1; i <= targetAreaX[1]; i++)
        {
            int triangleNumber = (i * (i + 1)) / 2;

            if (triangleNumber >= targetAreaX[0] && triangleNumber <= targetAreaX[1])
            {
                minX = i;
                break;
            }

            if (triangleNumber > targetAreaX[1])
                break;
        }

        for (int i = 1; i <= -targetAreaY[0]; i++)
        {
            int triangleNumber = (i * (i + 1)) / 2;

            for (int j = 1; ; j++)
            {
                int negativeTriangleNumber = ((j * (j + 1)) / 2) * -1;

                //what goes up must come down (and land in the target area)
                int difference = negativeTriangleNumber + triangleNumber;

                if (difference >= targetAreaY[0] && difference <= targetAreaY[1])
                {
                    if (triangleNumber >= maxY)
                    {
                        maxY = i;
                        maxHeight = triangleNumber;
                    }
                    else
                        break;
                }

                if (difference < targetAreaY[0])
                    break;
            }
        }

        Console.WriteLine(maxHeight);

        var possibleVelocities = 0;

        //I wonder if there's a more clever way to do this than running simulations for each combination of
        // velocities
        foreach (var x in Enumerable.Range(minX, targetAreaX[1] - minX + 1))
        {
            foreach (var y in Enumerable.Range(targetAreaY[0], maxY - targetAreaY[0] + 1))
            {
                var iterationX = x;
                var iterationY = y;
                var probePos = new int[] { 0, 0 };

                while (true)
                {
                    probePos[0] += iterationX;
                    probePos[1] += iterationY;

                    if (probePos[0] > targetAreaX[1] || probePos[1] < targetAreaY[0])
                        break;

                    if (iterationX != 0)
                        iterationX += iterationX > 0 ? -1 : 1;

                    iterationY -= 1;

                    if ((probePos[0] >= targetAreaX[0] && probePos[0] <= targetAreaX[1])
                     && (probePos[1] >= targetAreaY[0] && probePos[1] <= targetAreaY[1]))
                    {
                        possibleVelocities++;
                        break;
                    }
                }
            }
        }

        Console.WriteLine(possibleVelocities);
    }

}