namespace Advent2016;

public static class Day3
{
    public static void Run(string[] data)
    {
        int countPossible = data.Aggregate(0, (acc, line) =>
               {
                   int[] sides = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToArray();

                   if (sides[0] + sides[1] > sides[2]
                   && sides[1] + sides[2] > sides[0]
                   && sides[0] + sides[2] > sides[1])
                       acc++;

                   return acc;
               });

        Console.WriteLine(countPossible);

        countPossible = data.Chunk(3).Aggregate(0, (acc, chunck) =>
        {
            var line1 = chunck[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToArray();
            var line2 = chunck[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToArray();
            var line3 = chunck[2].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToArray();

            //Triage 1
            if (line1[0] + line2[0] > line3[0]
            && line1[0] + line3[0] > line2[0]
            && line2[0] + line3[0] > line1[0])
                acc++;

            //Triage 2
            if (line1[1] + line2[1] > line3[1]
            && line1[1] + line3[1] > line2[1]
            && line2[1] + line3[1] > line1[1])
                acc++;

            //Triage 3
            if (line1[2] + line2[2] > line3[2]
            && line1[2] + line3[2] > line2[2]
            && line2[2] + line3[2] > line1[2])
                acc++;

            return acc;
        });

        Console.WriteLine(countPossible);
    }
}