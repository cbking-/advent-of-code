using System.Collections;

namespace Advent2022;

public static class Day5
{
    public static void Run(string[] data)
    {
        //    [D]
        //[N] [C]
        //[Z] [M] [P]
        // 1   2   3
        //
        //move 1 from 2 to 1
        //move 3 from 1 to 3
        //move 2 from 2 to 1
        //move 1 from 1 to 2

        var endInitialCreates = new Regex(@"^(\s*(\d+)\s*)+$");

        var splitIndex = data.WithIndex()
                             .Where(line => endInitialCreates.IsMatch(line.Item))
                             .Select(line => line.Index)
                             .Single();

        var splitData = data.WithIndex()
                            .GroupBy(line => line.Index > splitIndex)
                            .Select(group => group.Select(line => line.Item));

        var numberOfStacks = int.Parse(endInitialCreates.Matches(splitData.First().Last())
                                              .Last().Groups[2].Value);

        var stacks = new Stack[numberOfStacks].Select(stack => new Stack()).ToArray();
        var betterStacks = new Stack[numberOfStacks].Select(stack => new Stack()).ToArray();

        splitData.First().Reverse().Skip(1).ToList().ForEach(line =>
        {
            line.Skip(1)
                .WithIndex()
                .Where(crate => crate.Index % 4 == 0)
                .ToList()
                .ForEach(crate =>
                {
                    if (crate.Item != ' ')
                    {
                        stacks[crate.Index / 4].Push(crate.Item);
                        betterStacks[crate.Index / 4].Push(crate.Item);
                    }
                });
        });

        splitData.Last().ToList().ForEach(move =>
        {
            var movePattern = new Regex(@"move (\d+) from (\d+) to (\d+)");
            var moveData = movePattern.Matches(move);

            var numberOfCrates = int.Parse(moveData.Single().Groups[1].Value);
            var fromStack = int.Parse(moveData.Single().Groups[2].Value);
            var toStack = int.Parse(moveData.Single().Groups[3].Value);

            foreach (var index in Enumerable.Range(0, numberOfCrates))
            {
                stacks[toStack - 1].Push(stacks[fromStack - 1].Pop());
            }

            var interStack = new Stack();

            foreach (var index in Enumerable.Range(0, numberOfCrates))
            {
                interStack.Push(betterStacks[fromStack - 1].Pop());
            }

            foreach (var index in Enumerable.Range(0, numberOfCrates))
            {
                betterStacks[toStack - 1].Push(interStack.Pop());
            }
        });

        StringBuilder builder = new();

        foreach (var stack in stacks)
        {
            builder.Append(stack.Pop());
        }

        Console.WriteLine(builder.ToString());

        builder = new();

        foreach (var stack in betterStacks)
        {
            builder.Append(stack.Pop());
        }

        Console.WriteLine(builder.ToString());
    }

}