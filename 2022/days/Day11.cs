using System.Data;

namespace Advent2022;

public class Monkey
{
    public Queue<long> Items = new();

    public long Inspects = 0;

    public Func<long, long> Inspect = (long item) => { return 0; };

    public Func<long, long> WorseInspect = (long item) => { return 0; };

    public Func<long, int> ThrowTo = (long item) => { return 0; };
}


public static class Day11
{
    public static void Run(string[] data)
    {
        List<Monkey> monkeys = new();

        //Had to look at solutions for this one
        //I don't completely understand the concept at work here
        var modulus = data.Chunk(6).Select(chunck => chunck.Where((item, index) => index % 3 == 0).Last().Split(' ', StringSplitOptions.RemoveEmptyEntries).Last())
                          .Aggregate(1, (acc, item) => int.Parse(item) * acc);

        Action SetUp = () =>
        {
            monkeys = new();
            foreach (var monkeyData in data.Chunk(6))
            {
                var monkey = new Monkey();
                var items = monkeyData.ElementAt(1).Substring(17).Split(", ", StringSplitOptions.RemoveEmptyEntries);
                monkey.Items = new Queue<long>(items.Select(item => long.Parse(item)));

                var calculation = monkeyData.ElementAt(2).Split('=').Last().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var operation = calculation.Skip(1).First();
                var rightOperand = calculation.Last();

                monkey.Inspect = (long item) =>
                {
                    if (rightOperand == "old")
                        item = Convert.ToInt64(new DataTable().Compute($"{item} {operation} {item}", null));
                    else
                        item = Convert.ToInt64(new DataTable().Compute($"{item} {operation} {rightOperand}", null));

                    return Convert.ToInt64(Math.Floor(item / 3.0));
                };

                monkey.WorseInspect = (long item) =>
                {
                    if (rightOperand == "old")
                        item = Convert.ToInt64(new DataTable().Compute($"{item}.0 {operation} {item}.0", null));
                    else
                        item = Convert.ToInt64(new DataTable().Compute($"{item}.0 {operation} {rightOperand}.0", null));

                    return item % modulus;
                };

                var divisor = long.Parse(monkeyData.ElementAt(3).Split(' ').Last());
                var trueReturn = int.Parse(monkeyData.ElementAt(4).Split(' ').Last());
                var falseReturn = int.Parse(monkeyData.ElementAt(5).Split(' ').Last());

                monkey.ThrowTo = (long item) =>
                {
                    if (item % divisor == 0)
                        return trueReturn;

                    return falseReturn;
                };

                monkeys.Add(monkey);
            }
        };

        SetUp();

        foreach (var round in Enumerable.Range(1, 20))
        {
            foreach (var monkey in monkeys)
            {
                while (monkey.Items.Any())
                {
                    var item = monkey.Items.Dequeue();

                    item = monkey.Inspect(item);

                    var monkeyToThrowTo = monkey.ThrowTo(item);

                    monkeys.ElementAt(monkeyToThrowTo).Items.Enqueue(item);

                    monkey.Inspects += 1;
                }
            }
        }

        Console.WriteLine(monkeys.OrderByDescending(monkey => monkey.Inspects)
                                 .Take(2)
                                 .Select(monkey => monkey.Inspects)
                                 .Aggregate((long)1, (num, acc) => num * acc));

        SetUp();

        foreach (var round in Enumerable.Range(1, 10000))
        {
            foreach (var monkey in monkeys)
            {
                while (monkey.Items.Any())
                {
                    var item = monkey.Items.Dequeue();

                    item = monkey.WorseInspect(item);

                    var monkeyToThrowTo = monkey.ThrowTo(item);

                    monkeys.ElementAt(monkeyToThrowTo).Items.Enqueue(item);

                    monkey.Inspects += 1;
                }
            }
        }

        Console.WriteLine(monkeys.OrderByDescending(monkey => monkey.Inspects)
                                 .Take(2)
                                 .Select(monkey => monkey.Inspects)
                                 .Aggregate((long)1, (num, acc) => num * acc));
    }

}