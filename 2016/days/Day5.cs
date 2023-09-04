namespace Advent2016;

public static class Day5
{
    public static void Run(string[] data)
    {
        string input = data[0];
        StringBuilder password = new();
        StringBuilder passwordTwo = new("________");

        foreach (int number in Enumerable.Range(0, int.MaxValue))
        {
            using MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes($"{input}{number}");
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            string hexValue = Convert.ToHexString(hashBytes);

            if (hexValue[..5] == "00000")
            {
                if (password.Length < 8)
                {
                    password.Append(hexValue[5]);
                }


                if (int.TryParse(char.ToString(hexValue[5]), out int result)
                    && result < 8
                    && passwordTwo[result] == '_')
                {
                    passwordTwo[result] = hexValue[6];
                }
            }

            if (password.Length == 8 && !passwordTwo.ToString().Contains('_'))
            {
                break;
            }
        }

        Console.WriteLine("Door one: " + password);
        Console.WriteLine("Door two: " + passwordTwo);
    }
}