namespace Advent2022;

public class Knot
{
    public Knot()
    {
        //instead of figuring out how to deal with the head and tail being -1 and +1
        // i'll just set the initial coordinates way out there so it's incredibly
        // unlikely negatives even come in to consideration.
        X = int.MaxValue / 2;
        Y = int.MaxValue / 2;

        Visited.Add($"{X},{Y}");
    }

    public int X;
    public int Y;
    public Knot? Child;
    public HashSet<string> Visited = new();
    public char Label = '0';
    public Knot GetTail()
    {
        if (Child is not null)
        {
            return Child.GetTail();
        }

        return this;
    }

    public void Draw()
    {
        var grid = Enumerable.Repeat('.', 30 * 30).ToArray();
        var knot = this;

        while (knot is not null)
        {
            grid[knot.X + knot.Y * 30] = knot.Label;
            knot = knot.Child;
        }

        Console.WriteLine(string.Join('\n', grid.Chunk(30).Reverse().Select(chunck => string.Join("", chunck))));
        Console.WriteLine("=================");
    }

    public void Move(string direction, int steps)
    {
        foreach (var step in Enumerable.Range(1, steps))
        {
            switch (direction)
            {
                case "U":
                    Y += 1;
                    break;

                case "R":
                    X += 1;
                    break;

                case "D":
                    Y -= 1;
                    break;

                case "L":
                    X -= 1;
                    break;
            }

            MoveChildren();

            //Draw();
        }
    }

    public void MoveChildren()
    {
        if (Child is null)
            return;

        if (Child.X - X == 2)
        {
            Child.X -= 1;

            if (Child.Y - Y > 0)
                Child.Y -= 1;

            if (Child.Y - Y < 0)
                Child.Y += 1;
        }
        else if (Child.X - X == -2)
        {
            Child.X += 1;

            if (Child.Y - Y > 0)
                Child.Y -= 1;

            if (Child.Y - Y < 0)
                Child.Y += 1;
        }
        else if (Child.Y - Y == 2)
        {
            Child.Y -= 1;

            if (Child.X - X > 0)
                Child.X -= 1;

            if (Child.X - X < 0)
                Child.X += 1;
        }
        else if (Child.Y - Y == -2)
        {
            Child.Y += 1;

            if (Child.X - X > 0)
                Child.X -= 1;

            if (Child.X - X < 0)
                Child.X += 1;
        }


        Child.Visited.Add($"{Child.X},{Child.Y}");
        Child.MoveChildren();
    }
}


public static class Day9
{
    public static void Run(string[] data)
    {
        var head = new Knot
        {
            Label = 'H'
        };
        var width = 40;

        Action Draw = () =>
        {
            var grid = Enumerable.Repeat('.', width * width).ToArray();
            var knot = head;

            while (knot is not null)
            {
                grid[knot.X + knot.Y * width] = knot.Label;
                knot = knot.Child;
            }

            Console.WriteLine(string.Join('\n', grid.Chunk(width).Reverse().Select(chunck => string.Join("", chunck))));
            Console.WriteLine("=================");
        };


        foreach (var index in Enumerable.Range(1, 9))
        {
            var child = new Knot();
            child.Label = char.Parse(index.ToString());
            head.GetTail().Child = child;
        }

        foreach (var line in data)
        {
            var instruction = line.Split(' ');
            var direction = instruction[0];
            var steps = int.Parse(instruction[1]);

            head.Move(direction, steps);

            //Draw();
        }

        //Console.WriteLine(string.Join('\n', head.GetTail().Visited));

        Console.WriteLine(head.Child?.Visited.Count);
        Console.WriteLine(head.GetTail().Visited.Count);
    }

}