namespace Advent2015;

public static class Day11
{
    public static void Run(string[] data)
    {
        var straightPattern = "(abc|bcd|cde|def|efg|fgh|pqr|qrs|rst|stu|tuv|uvw|vwx|wxy|xyz)";
        var invalidCharactersPattern = "[iol]";

        //This rule isn't super clear to me from the instructions on the site
        // Is "aabaa" supposed to be valid?
        // My best guess is aabaa is valid. "Non-overlapping" means something like "aaa" is invalid
        // "Two different" means positionally different (at least one character between pairs) not that the pairs are
        // different letters.
        var pairPairsPattern = @"(.)\1.*(.)\2";

        var isValid = false;
        var builder = new StringBuilder(data[0]);

        while (!isValid)
        {
            for (var index = builder.Length - 1; index > 0; index--)
            {
                if (builder[index] == 'z')
                {
                    builder[index] = 'a';
                }
                else
                {
                    builder[index] = (Char)(Convert.ToUInt16(builder[index]) + 1);
                    break;
                }
            }

            if (Regex.IsMatch(builder.ToString(), straightPattern)
                && !Regex.IsMatch(builder.ToString(), invalidCharactersPattern)
                && Regex.IsMatch(builder.ToString(), pairPairsPattern))
            {
                isValid = true;
            }
        }

        Console.WriteLine($"Part 1: {builder.ToString()}");

        isValid = false;

        while (!isValid)
        {
            for (var index = builder.Length - 1; index > 0; index--)
            {
                if (builder[index] == 'z')
                {
                    builder[index] = 'a';
                }
                else
                {
                    builder[index] = (Char)(Convert.ToUInt16(builder[index]) + 1);
                    break;
                }
            }

            if (Regex.IsMatch(builder.ToString(), straightPattern)
                && !Regex.IsMatch(builder.ToString(), invalidCharactersPattern)
                && Regex.IsMatch(builder.ToString(), pairPairsPattern))
            {
                isValid = true;
            }
        }

        Console.WriteLine($"Part 2: {builder.ToString()}");
    }

}