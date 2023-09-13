namespace Advent2016;

public static class Day8
{
    public static void Run(string[] data)
    {
        const int width = 50;
        const int height = 6;

        //we don't care about all the pixels, only the ones that are on
        List<int[]> pixelsOn = new();

        foreach (string instruction in data)
        {
            string[] command = instruction.Split(' ');

            if (command[0] == "rect")
            {
                string[] rectSize = command[1].Split('x');

                foreach (int x in Enumerable.Range(0, int.Parse(rectSize[0])))
                {
                    foreach (int y in Enumerable.Range(0, int.Parse(rectSize[1])))
                    {
                        pixelsOn.Add(new int[] { x, y });
                    }
                }
            }
            else
            {
                if (command[1] == "row")
                {
                    int rowToMove = int.Parse(command[2].Split('=')[1]);
                    int moveBy = int.Parse(command[4]);

                    var pixelsToMove = pixelsOn.Where(pixel => pixel[1] == rowToMove);

                    foreach(int[] pixel in pixelsToMove)
                    {
                        pixel[0] = (pixel[0] + moveBy) % width;
                    }
                }
                else
                {
                    //column
                    int colToMove = int.Parse(command[2].Split('=')[1]);
                    int moveBy = int.Parse(command[4]);

                    var pixelsToMove = pixelsOn.Where(pixel => pixel[0] == colToMove);

                    foreach(int[] pixel in pixelsToMove)
                    {
                        pixel[1] = (pixel[1] + moveBy) % height;
                    }
                }
            }

            Console.WriteLine(instruction);
            bool[] grid = Enumerable.Repeat(false, width * height).ToArray();
            foreach(int[] pixel in pixelsOn)
            {
                grid[pixel[0] + pixel[1] * width] = true;
            }
            var splitGrid = grid.Select((item, index) => new { Index = index, Value = item })
                    .GroupBy(item => item.Index / width)
                    .Select(item => item.Select(v => v.Value).ToList())
                    .ToList();

            foreach (var line in splitGrid)
            {
                foreach (var light in line)
                {
                    Console.Write(light ? "#" : ".");
                }
                Console.Write("\n");
            }
            Console.Write("\n==================\n");
        }
      
        Console.WriteLine(pixelsOn.Count);
    }
}