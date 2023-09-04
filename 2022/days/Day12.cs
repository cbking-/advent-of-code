namespace Advent2022;

public static class Day12
{
    public static void Run(string[] data)
    {
        var formattedData = data.SelectMany(line => Array.ConvertAll(line.ToArray(), character => (int)character)).ToList();
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

        Console.WriteLine(answer);
    }

}