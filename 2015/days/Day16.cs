namespace Advent2015;

public class Aunt
{
    [JsonIgnore]
    public int Number { get; set; } = 0;

    [JsonProperty("children")]
    public int? Children { get; set; }

    [JsonProperty("cats")]
    public int? Cats { get; set; }

    [JsonProperty("samoyeds")]
    public int? Samoyeds { get; set; }

    [JsonProperty("pomeranians")]
    public int? Pomeranians { get; set; }

    [JsonProperty("akitas")]
    public int? Akitas { get; set; }

    [JsonProperty("vizslas")]
    public int? Vizslas { get; set; }

    [JsonProperty("goldfish")]
    public int? Goldfish { get; set; }

    [JsonProperty("trees")]
    public int? Trees { get; set; }

    [JsonProperty("cars")]
    public int? Cars { get; set; }

    [JsonProperty("perfumes")]
    public int? Perfumes { get; set; }
}

public static class Day16
{
    public static void Run(string[] data)
    {
        var auntToFind = new Aunt
        {
            Children = 3,
            Cats = 7,
            Samoyeds = 2,
            Pomeranians = 3,
            Akitas = 0,
            Vizslas = 0,
            Goldfish = 5,
            Trees = 3,
            Cars = 2,
            Perfumes = 1
        };

        var aunts = new List<Aunt>();
        foreach (var line in data.Select((value, index) => new { index, value }))
        {
            //converting line to JSON so it's easily parsed to object
            var sue = Regex.Replace(Regex.Replace(line.value, @"Sue \d+:", ""), @"(?<=\s)(\w+)(?=:)", @"""$1""");

            sue = $"{{ {sue} }}";

            var aunt = JsonConvert.DeserializeObject<Aunt>(sue) ?? new Aunt();
            aunt.Number = line.index + 1;
            aunts.Add(aunt);
        }

        //https://stackoverflow.com/questions/31114892/using-linq-to-find-best-match-across-multiple-properties
        var grouping = aunts.GroupBy(aunt =>
            (aunt.Akitas == auntToFind.Akitas ? 1 : 0) +
            (aunt.Cars == auntToFind.Cars ? 1 : 0) +
            (aunt.Cats == auntToFind.Cats ? 1 : 0) +
            (aunt.Children == auntToFind.Children ? 1 : 0) +
            (aunt.Goldfish == auntToFind.Goldfish ? 1 : 0) +
            (aunt.Perfumes == auntToFind.Perfumes ? 1 : 0) +
            (aunt.Pomeranians == auntToFind.Pomeranians ? 1 : 0) +
            (aunt.Samoyeds == auntToFind.Samoyeds ? 1 : 0) +
            (aunt.Trees == auntToFind.Trees ? 1 : 0) +
            (aunt.Vizslas == auntToFind.Vizslas ? 1 : 0)
        );

        var maxCount = grouping.Max(x => x.Key);
        var resultSet = grouping.FirstOrDefault(x => x.Key == maxCount)?.Select(g => g).Single();
        Console.WriteLine($"Part 1: {resultSet?.Number}");

        grouping = aunts.GroupBy(aunt =>
            (aunt.Akitas == auntToFind.Akitas ? 1 : 0) +
            (aunt.Cars == auntToFind.Cars ? 1 : 0) +
            (aunt.Cats > auntToFind.Cats ? 1 : 0) +
            (aunt.Children == auntToFind.Children ? 1 : 0) +
            (aunt.Goldfish < auntToFind.Goldfish ? 1 : 0) +
            (aunt.Perfumes == auntToFind.Perfumes ? 1 : 0) +
            (aunt.Pomeranians < auntToFind.Pomeranians ? 1 : 0) +
            (aunt.Samoyeds == auntToFind.Samoyeds ? 1 : 0) +
            (aunt.Trees > auntToFind.Trees ? 1 : 0) +
            (aunt.Vizslas == auntToFind.Vizslas ? 1 : 0)
        );

        maxCount = grouping.Max(x => x.Key);
        resultSet = grouping.FirstOrDefault(x => x.Key == maxCount)?.Select(g => g).Single();
        Console.WriteLine($"Part 1: {resultSet?.Number}");
    }

}