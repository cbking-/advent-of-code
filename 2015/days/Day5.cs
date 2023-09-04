namespace Advent2015;

public static class Day5
{
    public static void Run(string[] data)
    {
        var niceStringRuleOne = "^(.*[aeiou].*){3,}$";
        var niceStringRuleTwo = "([a-z])\\1";
        var niceStringRuleThree = "^(?!.*(ab|cd|pq|xy)).*$";

        var niceStrings = data.Where(line => Regex.IsMatch(line, niceStringRuleOne)
                                            && Regex.IsMatch(line, niceStringRuleTwo)
                                            && Regex.IsMatch(line, niceStringRuleThree))
                                .Count();

        Console.WriteLine($"Part 1: {niceStrings}");

        niceStringRuleOne = "([a-z][a-z]).*\\1";
        niceStringRuleTwo = "([a-z])[a-z]\\1";

        niceStrings = data.Where(line => Regex.IsMatch(line, niceStringRuleOne)
                                        && Regex.IsMatch(line, niceStringRuleTwo))
                                .Count();

        Console.WriteLine($"Part 2: {niceStrings}");
    }
}