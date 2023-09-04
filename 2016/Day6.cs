using System.Text;
using Core;

public static class Day6
{
    public static void Run(string[] data)
    {
        StringBuilder messageOne = new();
        StringBuilder messageTwo = new();

        foreach(int column in Enumerable.Range(0, data[0]. Length))
        {
            var orderedData = data.GetColumn(column)
                               .GroupBy(character => character)
                               .OrderByDescending(group => group.Count());

            messageOne.Append(orderedData.First().Key);
            messageTwo.Append(orderedData.Last().Key);
        }

        Console.WriteLine(messageOne);
        Console.WriteLine(messageTwo);
    }
}