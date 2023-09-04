namespace Advent2015;

public static class Day4
{
    public static void Run(string[] data)
    {
        var answer = 0;

        while (true)
        {
            var md5 = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes($"{data[0]}{answer}")) ?? new byte[] { };

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
            var md5 = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes($"{data[0]}{answer}")) ?? new byte[] { };

            if (Convert.ToHexString(md5).StartsWith("000000"))
            {
                break;
            }

            answer += 1;
        }

        Console.WriteLine($"Part 2: {answer}");
    }
}