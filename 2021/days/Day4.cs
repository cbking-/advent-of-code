namespace Advent2021;

public class BingoBoard
{
    public List<BingoPlace> Places { get; set; } = new List<BingoPlace>();

    public int BoardWidth { get; set; } = 0;

    public bool HasBingo()
    {
        //check rows
        foreach (var row in Enumerable.Range(0, BoardWidth))
        {
            var MarkedCount = 0;

            foreach (var column in Enumerable.Range(0, BoardWidth))
            {
                MarkedCount += Places[column + row * BoardWidth].Marked ? 1 : 0;
            }

            if (MarkedCount == BoardWidth)
                return true;
        }

        //check columns
        foreach (var column in Enumerable.Range(0, BoardWidth))
        {
            var MarkedCount = 0;

            foreach (var row in Enumerable.Range(0, BoardWidth))
            {
                MarkedCount += Places[column + row * BoardWidth].Marked ? 1 : 0;
            }

            if (MarkedCount == BoardWidth)
                return true;
        }

        return false;
    }
}

public class BingoPlace
{
    public bool Marked { get; set; } = false;
    public int Number { get; set; } = 0;
}

public static class Day4
{
    public static void Run(string[] data)
    {
        var draws = Array.ConvertAll(data[0].Split(','), int.Parse);
        var partOne = 0;
        var partTwo = 0;
        var boards = new List<BingoBoard>();
        var boardWidth = 5;
        var board = new BingoBoard() { BoardWidth = boardWidth };

        foreach (var line in data.WithIndex().Skip(1))
        {
            var row = Array.ConvertAll(line.Item.Split(' ', StringSplitOptions.RemoveEmptyEntries), number => int.Parse(number));

            foreach (var number in row.WithIndex())
            {
                var location = (number.Index % 5) + ((line.Index % 5) - 1) * boardWidth;
                board.Places.Add(new BingoPlace { Number = number.Item });
            }

            if (line.Index % boardWidth == 0)
            {
                boards.Add(board);
                board = new BingoBoard() { BoardWidth = boardWidth };
            }
        }

        foreach (var draw in draws)
        {
            boards.ForEach(board => board.Places.Where(place => place.Number == draw).ToList().ForEach(place => place.Marked = true));

            if (boards.Any(board => board.HasBingo()))
            {
                if (partOne == 0)
                    partOne = boards.Where(board => board.HasBingo()).Single().Places.Where(place => !place.Marked).Sum(place => place.Number) * draw;

                if (boards.Count() == 1)
                    partTwo = boards.Single().Places.Where(place => !place.Marked).Sum(place => place.Number) * draw;

                boards.RemoveAll(board => board.HasBingo());
            }
        }

        Console.WriteLine($"Part 1: \x1b[93m{partOne}\x1b[0m");
        Console.WriteLine($"Part 2: \x1b[93m{partTwo}\x1b[0m");
    }

}