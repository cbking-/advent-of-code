namespace Advent2021;

public static class Day12
{
    public static void Run(string[] data)
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        var graph = new Graph();
        var paths = new List<List<string>>();

        foreach (var line in data)
        {
            var node = new GraphNode(line.Split('-')[0]);
            node.AddNeighbor(line.Split('-')[1]);
            graph.AddNode(node);

            node = new GraphNode(line.Split('-')[1]);
            node.AddNeighbor(line.Split('-')[0]);
            graph.AddNode(node);
        }

        Action<GraphNode, List<string>, bool> Visit = (node, currentPath, visitOneSmallTwice) => { };

        Visit = (node, currentPath, visitOneSmallTwice) =>
        {
            var iterationPath = new List<string>(currentPath);
            iterationPath.Add(node.Identifier);

            if (node.Identifier == "end")
            {
                paths.Add(iterationPath);
                return;
            }

            foreach (var neighbor in node.Neighbors)
            {
                if (neighbor[0] >= 97 && !iterationPath.Contains(neighbor)
                    || (neighbor[0] >= 97 && visitOneSmallTwice && iterationPath.GroupBy(path => path).Where(path => path.Key[0] >= 97).All(group => group.Count() < 2) && neighbor != "start"))
                {
                    Visit(graph.Nodes.Single(graphNode => graphNode.Identifier == neighbor), iterationPath, visitOneSmallTwice);
                }
                else if (neighbor[0] < 97)
                {
                    Visit(graph.Nodes.Single(graphNode => graphNode.Identifier == neighbor), iterationPath, visitOneSmallTwice);
                }
            }

            return;
        };

        Visit(graph.Nodes.Single(node => node.Identifier == "start"), new List<string>(), false);
        Console.WriteLine($"Part 1: \x1b[93m{paths.Count()}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 1 Execution Time: {watch.ElapsedMilliseconds} ms");

        watch.Reset();
        watch.Start();

        paths = new List<List<string>>();
        Visit(graph.Nodes.Single(node => node.Identifier == "start"), new List<string>(), true);

        Console.WriteLine($"Part 2: \x1b[93m{paths.Count()}\x1b[0m");
        watch.Stop();
        Console.WriteLine($"Part 2 Execution Time: {watch.ElapsedMilliseconds} ms");
    }

}