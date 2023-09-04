namespace Advent2015;

public static class Day13
{
    public static void Run(string[] data)
    {
        //At first, I thought this would be pretty similar to how I
        // solved Day 9. However, I found that the nearest neighbor algorithm
        // doesn't work for this problem as well as it did for the other.
        // I think it has to do with how each vertex has two neighbors (rather than a boolean 'visited')
        // and creates a circuit rather than a straight path.
        // I'm not smart enough to figure out how to get nearest neighbor to work here.

        //will maintain original values
        var vertices = new List<TSPVertex>();

        foreach (var line in data)
        {
            var pattern = @"(\w+) would (\w+) (\d+) happiness units by sitting next to (\w+)";
            var matches = Regex.Matches(line, pattern);

            var match = matches.First();

            var sign = match.Groups[2].Value == "gain" ? "+" : "-";

            var Distance = int.Parse(sign + match.Groups[3].Value);
            var City = match.Groups[1].Value;
            var Neighbor = match.Groups[4].Value;

            var vertex = vertices.Where(route => route.City == City).SingleOrDefault() ?? new TSPVertex { City = City, Visited = false };

            if (!vertices.Any(route => route.City == vertex.City))
            {
                vertex.Neighbors.Add(Neighbor, Distance);
                vertices.Add(vertex);
            }
            else
            {
                vertices.Where(route => route.City == City).Select(route => { route.Neighbors.Add(Neighbor, Distance); return route; }).ToList();
            }
        }

        //used to save calculated values
        var updatedVertices = JsonConvert.DeserializeObject<List<TSPVertex>>(JsonConvert.SerializeObject(vertices)) ?? new List<TSPVertex>();

        //sum vertex and neighbors' happiness values to get delta happiness
        foreach (var vertex in vertices)
        {
            foreach (var neighbor in vertex.Neighbors)
            {
                var neighborVertexHappiness = vertices.Where(vert => vert.City == neighbor.Key)
                                                      .Single()
                                                      .Neighbors
                                                      .Where(kvp => kvp.Key == vertex.City)
                                                      .Single()
                                                      .Value;

                updatedVertices.Where(vert => vert.City == vertex.City).Single().Neighbors[neighbor.Key] += neighborVertexHappiness;
            }
        }

        var happiness = 0;

        //nearest neighbor algorithm doesn't work for this one
        foreach (var permutation in new Permutations<TSPVertex>(updatedVertices))
        {
            var iterationHappiness = 0;

            foreach (var vertex in permutation)
            {
                iterationHappiness += vertex.Neighbors.Where(kvp => kvp.Key == GetNext(permutation, vertex)?.City).Single().Value;
            }

            happiness = Math.Max(happiness, iterationHappiness);
        }

        Console.WriteLine($"Part 1: {happiness}");

        updatedVertices.Add(new TSPVertex
        {
            City = "Corbin",
            Visited = false
        });

        foreach (var vertex in updatedVertices)
        {
            if (vertex.City != "Corbin")
            {
                vertex.Neighbors.Add("Corbin", 0);
                updatedVertices.Where(vert => vert.City == "Corbin").Single().Neighbors.Add(vertex.City, 0);
            }
        }

        happiness = 0;

        foreach (var permutation in new Permutations<TSPVertex>(updatedVertices))
        {
            var iterationHappiness = 0;

            foreach (var vertex in permutation)
            {
                iterationHappiness += vertex.Neighbors.Where(kvp => kvp.Key == GetNext(permutation, vertex)?.City).Single().Value;
            }

            happiness = Math.Max(happiness, iterationHappiness);
        }

        Console.WriteLine($"Part 2: {happiness}");
    }

}