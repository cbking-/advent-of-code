namespace Advent2015;

public static class Day12
{
    public static void Run(string[] data)
    {
        var sum = Regex.Matches(data[0], @"-?\d+").Sum(match => int.Parse(match.Value));
        Console.WriteLine($"Part 1: {sum}");

        //Newtonsoft's Json.NET is much easier to work with
        // in this case than System.Text.Json
        var json = JObject.Parse(data[0]);

        json.Descendants()
            .OfType<JObject>()
            .Where(obj => obj.Properties().Values().Any(val => val.Type == JTokenType.String && val.Value<string>() == "red"))
            .ToList()
            .ForEach(obj =>
            {
                try
                {
                    obj.Remove();
                }
                catch
                {
                    //Can't remove the value from a JProperty so remove the JProperty
                    obj.Parent?.Remove();
                }
            });

        var serialized = JsonConvert.SerializeObject(json);

        sum = Regex.Matches(serialized, @"-?\d+").Sum(match => int.Parse(match.Value));
        Console.WriteLine($"Part 2: {sum}");
    }

}