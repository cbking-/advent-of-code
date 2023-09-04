namespace Advent2021;

public static class Day15
{
    public static void Run(string[] data)
    {
        //Speed could be greatly improved using a better pathing algorithm.
        // Part one completes in 315 ms and part two take nearly 3 minutes
        // Not entirely sure where the performance can be improved but maybe
        // I'll figure it out some day.

        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        var formattedData = data.SelectMany(line => Array.ConvertAll(line.ToArray(), character => int.Parse(character.ToString()))).ToList();
        var graphSize = data[0].Length * data.Length;
        var graphWidth = data[0].Length;

        var graph = Enumerable.Range(0, graphSize).Select(i => (Enumerable.Repeat(new Dictionary<int, int>(), 4).ToArray())).ToArray();

        foreach (var vertex in formattedData.WithIndex())
        {
            var left = vertex.Index % graphWidth == 0 ? -1 : vertex.Index - 1;
            var right = (vertex.Index + 1) % graphWidth == 0 ? -1 : vertex.Index + 1;
            var bottom = (vertex.Index + graphWidth) >= graphSize ? -1 : vertex.Index + graphWidth;
            var top = (vertex.Index - graphWidth);

            if (top >= 0)
                graph[vertex.Index][0] = new Dictionary<int, int> { { top, formattedData.ElementAt(top) } };

            if (left >= 0)
                graph[vertex.Index][1] = new Dictionary<int, int> { { left, formattedData.ElementAt(left) } };

            if (right >= 0)
                graph[vertex.Index][2] = new Dictionary<int, int> { { right, formattedData.ElementAt(right) } };

            if (bottom >= 0)
                graph[vertex.Index][3] = new Dictionary<int, int> { { bottom, formattedData.ElementAt(bottom) } };
        }

        var answer = Helpers.DijkstraAlgo(graph);

        Console.WriteLine($"Part 1: \x1b[93m{answer.Last()}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 1 Execution Time: {watch.ElapsedMilliseconds} ms");

        watch.Reset();
        watch.Start();

        var newData = new List<int>(formattedData);
        var newGraphWidth = data[0].Length * 5;

        //create first grid row
        foreach (var row in Enumerable.Range(0, data.Length))
        {
            //generate second grid
            var additionalData = new List<int>(formattedData.Skip(row * graphWidth)
                                          .Take(graphWidth)
                                          .Select(index =>
                                          {
                                              if (index == 9)
                                                  return 1;

                                              return index + 1;
                                          }));

            foreach (var iteration in Enumerable.Range(0, 3))
            {
                additionalData.AddRange(additionalData.Skip(iteration * graphWidth)
                                          .Take(graphWidth)
                                          .Select(index =>
                                          {
                                              if (index == 9)
                                                  return 1;

                                              return index + 1;
                                          }));
            }

            var insertPoint = (row * (graphWidth + additionalData.Count())) + graphWidth;

            newData.InsertRange(insertPoint, additionalData);
        }

        //generate the rest of the rows
        foreach (var row in Enumerable.Range(0, data.Length * 4))
        {
            //generate second grid
            var additionalData = new List<int>(newData.Skip(row * newGraphWidth)
                                          .Take(newGraphWidth)
                                          .Select(index =>
                                          {
                                              if (index == 9)
                                                  return 1;

                                              return index + 1;
                                          }));

            newData.AddRange(additionalData);
        }

        var newGraphSize = newData.Count();

        graph = Enumerable.Range(0, newGraphSize).Select(i => (Enumerable.Repeat(new Dictionary<int, int>(), 4).ToArray())).ToArray();

        foreach (var vertex in newData.WithIndex())
        {
            var left = vertex.Index % newGraphWidth == 0 ? -1 : vertex.Index - 1;
            var right = (vertex.Index + 1) % newGraphWidth == 0 ? -1 : vertex.Index + 1;
            var bottom = (vertex.Index + newGraphWidth) >= newGraphSize ? -1 : vertex.Index + newGraphWidth;
            var top = (vertex.Index - newGraphWidth);

            if (top >= 0)
                graph[vertex.Index][0] = new Dictionary<int, int> { { top, newData.ElementAt(top) } };

            if (left >= 0)
                graph[vertex.Index][1] = new Dictionary<int, int> { { left, newData.ElementAt(left) } };

            if (right >= 0)
                graph[vertex.Index][2] = new Dictionary<int, int> { { right, newData.ElementAt(right) } };

            if (bottom >= 0)
                graph[vertex.Index][3] = new Dictionary<int, int> { { bottom, newData.ElementAt(bottom) } };
        }

        answer = Helpers.DijkstraAlgo(graph);

        Console.WriteLine($"Part 2: \x1b[93m{answer.Last()}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 2 Execution Time: {watch.ElapsedMilliseconds} ms");
    }

}