using System.Diagnostics.CodeAnalysis;

namespace Core;
public static class Helpers
{
    public static async Task<string[]> LoadDataAsync(string day)
    {
        using var file = new StreamReader($"inputs{Path.DirectorySeparatorChar}{day}.txt");
        var data = await file.ReadToEndAsync();
        return data.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
    }

    //https://stackoverflow.com/a/14663233/17400290
    public class IntArrayKeyComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[]? x, int[]? y)
        {
            if (x?.Length != y?.Length)
            {
                return false;
            }
            for (int i = 0; i < x?.Length; i++)
            {
                if (x[i] != y?[i])
                {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(int[] obj)
        {
            int result = 17;
            for (int i = 0; i < obj.Length; i++)
            {
                unchecked
                {
                    result = result * 23 + obj[i];
                }
            }
            return result;
        }
    }

    //based on http://www.herlitz.nu/2011/12/01/getting-the-previous-and-next-record-from-list-using-linq/
    ///<summary>
    /// Gets the next value or, if at the end of the list, gets the first value.
    ///</summary>
    public static T? GetNext<T>(IEnumerable<T> list, T current)
    {
        try
        {
            return list.SkipWhile(x => x != null && !x.Equals(current)).Skip(1).FirstOrDefault() ?? list.First();
        }
        catch
        {
            return default(T);
        }
    }

    //based on http://www.herlitz.nu/2011/12/01/getting-the-previous-and-next-record-from-list-using-linq/
    ///<summary>
    /// Gets the previous value or, if at the beginning of the list, gets the last value.
    ///</summary>
    public static T? GetPrevious<T>(IEnumerable<T> list, T current)
    {
        try
        {
            return list.TakeWhile(x => x != null && !x.Equals(current)).LastOrDefault() ?? list.Last();
        }
        catch
        {
            return default(T);
        }
    }

    public static IEnumerable<(T Item, int Index)> WithIndex<T>(this IEnumerable<T> source)
    {
        return source.Select((Item, Index) => (Item, Index));
    }

    public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int maximumItems)
    {
        return source.WithIndex().GroupBy(item => item.Index / maximumItems).Select(group => group.Select(item => item.Item));
    }
    //https://codereview.stackexchange.com/a/237442
    /// <summary>
    /// Finds all the divisors of any positive integer passed as argument.
    /// Returns an array of int with all the divisors of the argument.
    /// Returns null if the argument is zero or negative.
    /// </summary>
    public static int[] GetDivisors(int n)
    {
        if (n <= 0)
        {
            return new int[] { };
        }

        List<int> divisors = new List<int>();

        for (int i = 1; i <= Math.Sqrt(n); i++)
        {
            if (n % i == 0)
            {
                divisors.Add(i);
                if (i != n / i)
                {
                    divisors.Add(n / i);
                }
            }
        }

        divisors.Sort();

        return divisors.ToArray();
    }

    //https://stackoverflow.com/a/66848613/17400290
    public static T[][] Repeat<T>(this T[] arr, int count)
    {
        var res = new T[count][];
        for (int i = 0; i < count; i++)
        {
            //arr.CopyTo(res[i], 0);
            res[i] = (T[])arr.Clone();
        }
        return res;
    }

    private static int MinimumDistance(int[] distance, bool[] shortestPathTreeSet, int verticesCount)
    {
        int min = int.MaxValue;
        int minIndex = 0;

        for (int v = 0; v < verticesCount; ++v)
        {
            if (shortestPathTreeSet[v] == false && distance[v] <= min)
            {
                min = distance[v];
                minIndex = v;
            }
        }

        return minIndex;
    }

    public static int[] DijkstraAlgo(Dictionary<int, int>[][] graph)
    {
        var vertices = graph.Length;
        var neighbors = graph[0].Length;

        int[] distance = new int[vertices];
        bool[] shortestPathTreeSet = new bool[vertices];

        for (int i = 0; i < vertices; ++i)
        {
            distance[i] = int.MaxValue;
            shortestPathTreeSet[i] = false;
        }

        distance[0] = 0;

        for (int count = 0; count < vertices - 1; ++count)
        {
            int u = MinimumDistance(distance, shortestPathTreeSet, vertices);
            shortestPathTreeSet[u] = true;

            for (int neighbor = 0; neighbor < neighbors; ++neighbor)
            {
                var vertex = graph[u][neighbor].Keys.SingleOrDefault() == 0 ? -1 : graph[u][neighbor].Keys.Single();

                if (vertex == -1)
                    continue;

                if (!shortestPathTreeSet[vertex]
                    && Convert.ToBoolean(graph[u][neighbor].Values.SingleOrDefault())
                    && distance[u] != int.MaxValue
                    && distance[u] + graph[u][neighbor].Values.Single() < distance[vertex])
                {
                    distance[vertex] = distance[u] + graph[u][neighbor].Values.Single();
                }
            }
        }

        return distance;
    }
}

public class Graph
{
    public List<GraphNode> Nodes { get; set; } = new List<GraphNode>();

    public Graph() { }

    public Graph(List<GraphNode> _nodes)
    {
        Nodes = _nodes;
    }

    public void AddNode(GraphNode node)
    {
        if (Nodes.Contains(node))
        {
            Nodes.Single(_node => _node.Identifier == node.Identifier).AddNeighbors(node.Neighbors);
        }
        else
        {
            Nodes.Add(node);
        }
    }
}

public class GraphNode : IEquatable<GraphNode>
{
    public string Identifier { get; set; }

    public List<string> Neighbors { get; set; } = new List<string>();

    public GraphNode(string identifier)
    {
        Identifier = identifier;
    }

    public void AddNeighbor(string neighbor)
    {
        if (!Neighbors.Contains(neighbor))
            Neighbors.Add(neighbor);
    }

    public void AddNeighbors(List<string> neighbors)
    {
        Neighbors.AddRange(neighbors.Except(Neighbors));
    }

    public bool Equals(GraphNode? other)
    {
        if (other is null)
            return false;

        return Identifier == other.Identifier;
    }
}
