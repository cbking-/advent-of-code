namespace Advent2015;

public class Ingredient
{
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; } = 0;
    public int Durability { get; set; } = 0;
    public int Flavor { get; set; } = 0;
    public int Texture { get; set; } = 0;
    public int Calories { get; set; } = 0;
}


public static class Day15
{
    public static void Run(string[] data)
    {
        var ingredients = new List<Ingredient>();

        foreach (var line in data)
        {
            //Sugar: capacity 3, durability 0, flavor 0, texture -3, calories 2
            var pattern = @"(\w+): capacity (-?\d+), durability (-?\d+), flavor (-?\d+), texture (-?\d+), calories (-?\d+)";
            var match = Regex.Matches(line, pattern).First();

            ingredients.Add(new Ingredient
            {
                Name = match.Groups[1].Value,
                Capacity = int.Parse(match.Groups[2].Value),
                Durability = int.Parse(match.Groups[3].Value),
                Flavor = int.Parse(match.Groups[4].Value),
                Texture = int.Parse(match.Groups[5].Value),
                Calories = int.Parse(match.Groups[6].Value)
            });
        }

        var combinations = new Combinations<Ingredient>(ingredients, 100, GenerateOption.WithRepetition);
        var bestScore = 0;
        var bestScoreWithCalories = 0;

        foreach (var combo in combinations)
        {
            var groups = combo.GroupBy(ingredient => ingredient.Name);
            var capacity = Math.Clamp(groups.Sum(group => group.Sum(ingredient => ingredient.Capacity)), 0, int.MaxValue);
            var durability = Math.Clamp(groups.Sum(group => group.Sum(ingredient => ingredient.Durability)), 0, int.MaxValue);
            var flavor = Math.Clamp(groups.Sum(group => group.Sum(ingredient => ingredient.Flavor)), 0, int.MaxValue);
            var texture = Math.Clamp(groups.Sum(group => group.Sum(ingredient => ingredient.Texture)), 0, int.MaxValue);

            var calories = groups.Sum(group => group.Sum(ingredient => ingredient.Calories));

            if (calories == 500)
            {
                bestScoreWithCalories = Math.Max(bestScoreWithCalories, capacity * durability * flavor * texture);
            }

            bestScore = Math.Max(bestScore, capacity * durability * flavor * texture);
        }

        Console.WriteLine($"Part 1: {bestScore}");
        Console.WriteLine($"Part 2: {bestScoreWithCalories}");
    }
}