namespace Advent2015;

public class TSPVertex : IComparable
{
    public string City { get; set; } = string.Empty;

    public bool Visited { get; set; } = false;

    public Dictionary<string, int> Neighbors { get; set; } = new Dictionary<string, int>();

    public int CompareTo(object? obj)
    {
        if (obj == null) return 1;

        TSPVertex? otherVertex = obj as TSPVertex;
        if (otherVertex != null)
            return this.City.CompareTo(otherVertex.City);
        else
            throw new ArgumentException("Object is not a vertex");
    }
}


public static class Day9
{
    public static void Run(string[] data)
    {
        var vertices = new List<TSPVertex>();

        foreach (var line in data)
        {
            var pattern = @"(\w+) to (\w+) = (\d+)";
            var matches = Regex.Matches(line, pattern);

            var Distance = int.Parse(matches.First().Groups[3].Value);
            var City = matches.First().Groups[1].Value;
            var Neighbor = matches.First().Groups[2].Value;

            var vertex = vertices.Where(route => route.City == City).SingleOrDefault() ?? new TSPVertex { City = City, Visited = false };
            var neightborVertex = vertices.Where(route => route.City == Neighbor).SingleOrDefault() ?? new TSPVertex { City = Neighbor, Visited = false };

            if (!vertices.Any(route => route.City == vertex.City))
            {
                vertex.Neighbors.Add(Neighbor, Distance);
                vertices.Add(vertex);
            }
            else
            {
                vertices.Where(route => route.City == City).Select(route => { route.Neighbors.Add(Neighbor, Distance); return route; }).ToList();
            }

            //Also add neighbor vertex
            if (!vertices.Any(route => route.City == neightborVertex.City))
            {
                neightborVertex.Neighbors.Add(City, Distance);
                vertices.Add(neightborVertex);
            }
            else
            {
                vertices.Where(route => route.City == Neighbor).Select(route => { route.Neighbors.Add(City, Distance); return route; }).ToList();
            }

        }

        //Traveling Sales Person Problem
        //nearest neightbor algorithm

        // 1. Initialize all vertices as unvisited.
        // 2. Select an arbitrary vertex, set it as the current vertex u. Mark u as visited.
        // 3. Find out the shortest edge connecting the current vertex u and an unvisited vertex v.
        // 4. Set v as the current vertex u. Mark v as visited.
        // 5. If all the vertices in the domain are visited, then terminate. Else, go to step 3.

        var distance = int.MaxValue;

        //using each vertex as a staring point
        foreach (var vertex in vertices)
        {
            var currentVertex = vertex;
            var iterationDistance = 0;

            //loop until ever vertext has been visisted
            while (vertices.Any(vertex => vertex.Visited == false))
            {
                //null check so c# stops complaining
                if (currentVertex is null)
                {
                    break;
                }

                currentVertex.Visited = true;
                var nearestNeighbor = currentVertex.Neighbors.Where(neighbor => vertices.Any(vertex => vertex.City == neighbor.Key && !vertex.Visited))
                                                             .OrderBy(kvp => kvp.Value).FirstOrDefault();
                iterationDistance += nearestNeighbor.Value;
                currentVertex = vertices.Where(vertex => vertex.City == nearestNeighbor.Key && !vertex.Visited).SingleOrDefault();
            }

            distance = Math.Min(distance, iterationDistance);

            //reset visitation
            vertices.Select(vertex => { vertex.Visited = false; return vertex; }).ToList();
        }

        Console.WriteLine($"Part 1: {distance}");

        distance = 0;

        foreach (var vertex in vertices)
        {
            var currentVertex = vertex;
            var iterationDistance = 0;

            while (vertices.Any(vertex => vertex.Visited == false))
            {
                if (currentVertex is null)
                {
                    break;
                }

                currentVertex.Visited = true;
                var nearestNeighbor = currentVertex.Neighbors.Where(neighbor => vertices.Any(vertex => vertex.City == neighbor.Key && !vertex.Visited))
                                                             .OrderByDescending(kvp => kvp.Value).FirstOrDefault();
                iterationDistance += nearestNeighbor.Value;
                currentVertex = vertices.Where(vertex => vertex.City == nearestNeighbor.Key && !vertex.Visited).SingleOrDefault();
            }

            distance = Math.Max(distance, iterationDistance);

            vertices.Select(vertex => { vertex.Visited = false; return vertex; }).ToList();
        }

        Console.WriteLine($"Part 2: {distance}");
    }

}