namespace Advent2016;

public static class Day7
{
    public static void Run(string[] data)
    {
        int[] validIPs = data.Aggregate(new int[]{0,0}, (acc, line) =>
        {
            List<string> outsideBrackets = new();
            List<string> insideBrackets = new();

            StringBuilder address = new(line);

            while (true)
            {
                string tempAddress = address.ToString();
                int openIndex = tempAddress.IndexOf('[');
                int closeIndex = tempAddress.IndexOf(']');

                string outsideAddress = tempAddress;
                string insideAddress = string.Empty;

                if (openIndex > -1)
                {
                    outsideAddress = tempAddress[..openIndex];
                }

                outsideBrackets.Add(outsideAddress);

                if (closeIndex > -1)
                {
                    insideAddress = tempAddress[(openIndex + 1)..closeIndex];
                    insideBrackets.Add(insideAddress);
                }

                address.Remove(0, outsideAddress.Length + insideAddress.Length + (openIndex > -1 ? 2 : 0));

                if (address.Length == 0)
                {
                    break;
                }
            }

            bool isTLS = outsideBrackets.Any(supernet =>
                        {
                            var match = Regex.Match(supernet, @"(.)(.)\2\1");
                            return match.Length > 0 && match.Groups[1].Value != match.Groups[2].Value;
                        }) 
                        && insideBrackets.All(hypernet =>
                        {
                            var match = Regex.Match(hypernet, @"(.)(.)\2\1");
                            return !(match.Length > 0 && match.Groups[1].Value != match.Groups[2].Value);
                        });

            List<string> abas = outsideBrackets.SelectMany(supernet => {
                var matches = Regex.Matches(supernet, @"(?=((.)(.)\2))");
                return matches.Where(match => match.Groups[2].Value != match.Groups[3].Value)
                              .Select(match => match.Groups[1].Value);
            }).ToList();

            List<string> babs = insideBrackets.SelectMany(supernet => {
                var matches = Regex.Matches(supernet, @"(?=((.)(.)\2))");
                return matches.Where(match => match.Groups[2].Value != match.Groups[3].Value)
                              .Select(match => match.Groups[1].Value);
            }).ToList();

            bool isSSL = abas.Any(aba => babs.Any(bab => aba[0] == bab[1] && aba[1] == bab[0] && aba[1] == bab[2]));

            if (isTLS)
            {
                acc[0]++;
            }

            if(isSSL)
            {
                acc[1]++;
            }

            return acc;
        });

        Console.WriteLine(validIPs[0]);
        Console.WriteLine(validIPs[1]);
    }
}