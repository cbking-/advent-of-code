using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

var adventType = typeof(AdventOfCode);

var dayToRun = adventType.GetMethod(args[0], BindingFlags.Static | BindingFlags.Public) ?? throw new ArgumentException("Invalid day");

dynamic? task = dayToRun.Invoke(null, null);
await task;

public static class AdventOfCode
{
    public static async Task<String> LoadDataAsync(string fileName)
    {
        using var file = new StreamReader(fileName);
        return await file.ReadToEndAsync();
    }

    //https://stackoverflow.com/a/14663233/17400290
    public class IntArrayKeyComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[]? x, int[]? y)
        {
            if (x?.Length != y?.Length)
            {
                return false;
            }
            for (int i = 0; i < x?.Length; i++)
            {
                if (x[i] != y?[i])
                {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(int[] obj)
        {
            int result = 17;
            for (int i = 0; i < obj.Length; i++)
            {
                unchecked
                {
                    result = result * 23 + obj[i];
                }
            }
            return result;
        }
    }

    public static async Task Day1()
    {
        var data = await LoadDataAsync("input1");

        var answer = (data.Where(character => character == '(').Count()) - (data.Where(character => character == ')').Count());

        Console.WriteLine($"Part 1: {answer}");

        var floor = 0;
        answer = 0;

        foreach (var character in data)
        {
            if (floor == -1)
                break;

            answer += 1;

            floor += (character == '(' ? 1 : -1);
        }

        Console.WriteLine($"Part 2: {answer}");
    }

    public static async Task Day2()
    {
        var data = await LoadDataAsync("input2");
        var fixedData = data.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

        var answer = fixedData.Select(row => row.Split('x'))
                                  .Sum(dimension =>
                                  {
                                      var length = int.Parse(dimension[0]);
                                      var width = int.Parse(dimension[1]);
                                      var height = int.Parse(dimension[2]);

                                      var sideOne = length * width;
                                      var sideTwo = width * height;
                                      var sideThree = length * height;
                                      var smallSide = (new[] { sideOne, sideTwo, sideThree }).Min();

                                      return (2 * sideOne) + (2 * sideTwo) + (2 * sideThree) + smallSide;
                                  });

        Console.WriteLine($"Part 1: {answer}");

        answer = fixedData.Select(row => row.Split('x'))
                                  .Sum(dimension =>
                                  {
                                      var length = int.Parse(dimension[0]);
                                      var width = int.Parse(dimension[1]);
                                      var height = int.Parse(dimension[2]);
                                      var smallestPerim = (new[] { length, width, height }).OrderBy(num => num).Take(2).Sum() * 2;

                                      var volume = length * width * height;

                                      return smallestPerim + volume;
                                  });

        Console.WriteLine($"Part 2: {answer}");
    }

    public static async Task Day3()
    {
        var data = await LoadDataAsync("input3");

        var houses = new HashSet<int[]>(new IntArrayKeyComparer());
        var x = 0;
        var y = 0;
        houses.Add(new int[] { x, y });

        foreach (var character in data)
        {
            if (character == '^')
                y -= 1;

            if (character == 'v')
                y += 1;

            if (character == '<')
                x -= 1;

            if (character == '>')
                x += 1;

            houses.Add(new int[] { x, y });
        }

        Console.WriteLine($"Part 1: {houses.Count()}");

        houses = new HashSet<int[]>(new IntArrayKeyComparer());
        x = 0;
        y = 0;
        houses.Add(new int[] { x, y });

        foreach (var character in data.Where((character, index) => index % 2 == 0))
        {
            if (character == '^')
                y -= 1;

            if (character == 'v')
                y += 1;

            if (character == '<')
                x -= 1;

            if (character == '>')
                x += 1;

            houses.Add(new int[] { x, y });
        }

        x = 0;
        y = 0;

        foreach (var character in data.Where((character, index) => index % 2 != 0))
        {
            if (character == '^')
                y -= 1;

            if (character == 'v')
                y += 1;

            if (character == '<')
                x -= 1;

            if (character == '>')
                x += 1;

            houses.Add(new int[] { x, y });
        }

        Console.WriteLine($"Part 2: {houses.Count()}");
    }

    public static async Task Day4()
    {
        var data = await LoadDataAsync("input4");

        var answer = 0;

        while (true)
        {
            var md5 = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes($"{data}{answer}")) ?? new byte[] { };

            if (Convert.ToHexString(md5).StartsWith("00000"))
            {
                break;
            }
            answer += 1;
        }

        Console.WriteLine($"Part 1: {answer}");

        answer = 0;
        while (true)
        {
            var md5 = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes($"{data}{answer}")) ?? new byte[] { };

            if (Convert.ToHexString(md5).StartsWith("000000"))
            {
                break;
            }

            answer += 1;
        }

        Console.WriteLine($"Part 2: {answer}");
    }

    public static async Task Day5()
    {
        var data = await LoadDataAsync("input5");

        var lines = data.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

        var niceStringRuleOne = "^(.*[aeiou].*){3,}$";
        var niceStringRuleTwo = "([a-z])\\1";
        var niceStringRuleThree = "^(?!.*(ab|cd|pq|xy)).*$";

        var niceStrings = lines.Where(line =>  Regex.IsMatch(line, niceStringRuleOne) 
                                            && Regex.IsMatch(line, niceStringRuleTwo) 
                                            && Regex.IsMatch(line, niceStringRuleThree) )
                                .Count();

        Console.WriteLine($"Part 1: {niceStrings}");

        niceStringRuleOne = "([a-z][a-z]).*\\1";
        niceStringRuleTwo = "([a-z])[a-z]\\1";

        niceStrings = lines.Where(line =>  Regex.IsMatch(line, niceStringRuleOne) 
                                        && Regex.IsMatch(line, niceStringRuleTwo))
                                .Count();

        Console.WriteLine($"Part 2: {niceStrings}");
    }
}
