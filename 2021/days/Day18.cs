namespace Advent2021;

public class Pair
{
    public Pair? Parent { get; set; }
    public dynamic? Left { get; set; }
    public dynamic? Right { get; set; }

    public int Magnitude()
    {
        var leftMagnitude = 0;
        var rightMagnitude = 0;

        if (Left is Pair)
            leftMagnitude += Left.Magnitude() * 3;
        else
            leftMagnitude += Left * 3;

        if (Right is Pair)
            rightMagnitude += Right.Magnitude() * 2;
        else
            rightMagnitude += Right * 2;

        return leftMagnitude + rightMagnitude;
    }

    public override string ToString()
    {
        return $"[{Left},{Right}]";
    }

}

public static class Day18
{
    public static void Run(string[] data)
    {
        /*
            This is kind of ugly but it runs fast (on my computer lul) and gets the right
            answers. Haven't looked at other solutions yet but I'm sure there's some more
            elegant string parsing do be done and better binary tree solutions than what
            I managed to cobble together here. The Combinatorics package helped a lot
            for part 2. It's nice that I picked that up when doing 2015.
        */

        Func<IEnumerable<string>, List<Pair>> ParseData = (IEnumerable<string> input) =>
        {
            var output = new List<Pair>();

            foreach (var line in input)
            {
                var stack = new Stack<Pair>();
                Pair snailfishNumber = new Pair();

                foreach (var character in line)
                {
                    switch (character)
                    {
                        case '[':
                            snailfishNumber = new Pair();
                            stack.Push(snailfishNumber);
                            break;
                        case ']':
                            snailfishNumber = stack.Pop();

                            if (stack.Count > 0)
                            {
                                snailfishNumber.Parent = stack.Peek();

                                if (snailfishNumber.Parent.Left is null)
                                    snailfishNumber.Parent.Left = snailfishNumber;
                                else
                                    snailfishNumber.Parent.Right = snailfishNumber;

                                snailfishNumber = snailfishNumber.Parent;
                            }
                            break;
                        case ',':
                            continue;
                        default:
                            if (snailfishNumber.Left is null)
                                snailfishNumber.Left = int.Parse(character.ToString());
                            else
                                snailfishNumber.Right = int.Parse(character.ToString());
                            break;
                    }
                }

                output.Add(snailfishNumber);
            }

            return output;
        };

        Func<List<Pair>, int> Solve = (List<Pair> ToSolve) =>
        {
            var answer = ToSolve.Skip(1).Aggregate(ToSolve.First(), (acc, number) =>
        {
            // Console.WriteLine("  " + acc);
            // Console.WriteLine($"+ {number}");

            var newTree = new Pair { Left = acc, Right = number };

            newTree.Left.Parent = newTree;
            newTree.Right.Parent = newTree;


            //all number are normalized when adding
            // a split won't take a number above 5
            Func<Pair, Pair> FindPairToExplode = (Pair pair) => new Pair();

            FindPairToExplode = (Pair pair) =>
            {
                var queue = new Queue<Pair>();
                queue.Enqueue(pair);
                Pair node = null;
                var depth = 0;

                while (queue.Count > 0)
                {
                    var size = queue.Count;

                    for (int i = 0; i < size; i++)
                    {
                        node = queue.Dequeue();
                        if (node.Right is Pair)
                            queue.Enqueue(node.Right);
                        if (node.Left is Pair)
                            queue.Enqueue(node.Left);
                    }

                    depth += 1;
                }

                if (depth < 5)
                    return null;

                return node;
            };

            Func<Pair, Pair> FindPairToSplit = (Pair pair) => new Pair();

            FindPairToSplit = (Pair pair) =>
            {
                var stack = new Stack<Pair>();
                stack.Push(pair);
                Pair node = null;

                while (stack.Count > 0)
                {
                    node = stack.Pop();

                    if (node.Left is int && node.Left >= 10)
                        return node;

                    if ((node.Right is int && node.Right >= 10))
                    {
                        // [[9,10],20] will find 20
                        // [[14,0],25] will find 25
                        // without this
                        if (node.Left is Pair)
                        {
                            var leftNode = FindPairToSplit(node.Left);

                            if (leftNode is not null)
                                return leftNode;
                        }

                        return node;
                    }

                    if (node.Right is Pair)
                        stack.Push(node.Right);

                    if (node.Left is Pair)
                        stack.Push(node.Left);
                }

                return null;
            };

            while (true)
            {
                var pairToExplode = FindPairToExplode(newTree);

                while (pairToExplode is not null)
                {
                    var AddLeftNeighbor = (Pair pair) =>
                    {
                        if (pair is null)
                            return;

                        var valueToAdd = pair.Left;

                        while (pair.Parent is not null)
                        {
                            if (pair.Parent.Left is int)
                            {
                                pair.Parent.Left += valueToAdd;
                                break;
                            }

                            if (pair.Parent.Left != pair)
                            {
                                if (pair.Parent.Left is Pair)
                                {
                                    pair = pair.Parent.Left;

                                    while (pair.Right is Pair)
                                    {
                                        pair = pair.Right;
                                    }

                                    pair.Right += valueToAdd;
                                    break;
                                }
                                else
                                {
                                    pair.Parent.Left += valueToAdd;
                                    break;
                                }
                            }

                            pair = pair.Parent;
                        }
                    };

                    AddLeftNeighbor(pairToExplode);

                    var AddRightNeighbor = (Pair pair) =>
                    {
                        if (pair is null)
                            return;

                        var valueToAdd = pair.Right;

                        while (pair.Parent is not null)
                        {
                            if (pair.Parent.Right is int)
                            {
                                pair.Parent.Right += valueToAdd;
                                break;
                            }

                            if (pair.Parent.Right != pair)
                            {
                                if (pair.Parent.Right is Pair)
                                {
                                    pair = pair.Parent.Right;

                                    while (pair.Left is Pair)
                                    {
                                        pair = pair.Left;
                                    }

                                    pair.Left += valueToAdd;
                                    break;
                                }
                                else
                                {
                                    pair.Parent.Right += valueToAdd;
                                    break;
                                }
                            }

                            pair = pair.Parent;
                        }
                    };

                    AddRightNeighbor(pairToExplode);

                    if (pairToExplode.Parent.Left is Pair && pairToExplode == pairToExplode.Parent.Left)
                    {
                        pairToExplode.Parent.Left = 0;
                    }
                    else
                        pairToExplode.Parent.Right = 0;

                    //Console.WriteLine($"exploded:{newTree}");

                    pairToExplode = FindPairToExplode(newTree);
                }

                var pairToSplit = FindPairToSplit(newTree);

                while (pairToSplit is not null)
                {
                    if (pairToSplit.Left is int valueToSplit && valueToSplit >= 10)
                    {
                        var leftNumber = (int)Math.Floor(valueToSplit / 2.0);
                        var rightNumber = (int)Math.Ceiling(valueToSplit / 2.0);

                        pairToSplit.Left = new Pair()
                        {
                            Left = leftNumber,
                            Right = rightNumber,
                            Parent = pairToSplit
                        };
                    }
                    else
                    {
                        valueToSplit = pairToSplit.Right;

                        var leftNumber = (int)Math.Floor(valueToSplit / 2.0);
                        var rightNumber = (int)Math.Ceiling(valueToSplit / 2.0);

                        pairToSplit.Right = new Pair()
                        {
                            Left = leftNumber,
                            Right = rightNumber,
                            Parent = pairToSplit
                        };
                    }

                    //Console.WriteLine($"splitted:{newTree}");
                    pairToExplode = FindPairToExplode(newTree);

                    //need to exit if we've created an explodable pair
                    if (pairToExplode is not null)
                        break;

                    pairToSplit = FindPairToSplit(newTree);
                }

                if (pairToExplode is null && pairToSplit is null)
                    break;
            }

            // Console.WriteLine($"= {newTree}\n");
            return newTree;
        });

            return answer.Magnitude();
        };

        Console.WriteLine(Solve(ParseData(data)));

        var combos = new Combinations<string>(data, 2);
        var maxMagnitude = 0;

        foreach (var combo in combos)
        {
            var magnitude = Solve(ParseData(combo));

            maxMagnitude = magnitude > maxMagnitude ? magnitude : maxMagnitude;
        }

        Console.WriteLine(maxMagnitude);
    }
}