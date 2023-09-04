namespace Advent2015;

public static class Day23
{
    public static void Run(string[] data)
    {
        //initially was using int but part 2 was causing an overflow
        var registers = new Dictionary<string, long>() { { "a", 0 }, { "b", 0 } };

        Func<long, long> HLF = register => { return register >> 1; };
        Func<long, long> TPL = register => { return register * 3; };
        Func<long, long> INC = register => { return register + 1; };
        Func<long, bool> JIE = register => { return register % 2 == 0; };
        Func<long, bool> JIO = register => { return register == 1; };

        for (int i = 0; i < data.Length; i++)
        {
            var instruction = data[i].Substring(0, 3);
            var register = "";
            var offset = 0;

            if (instruction != "jmp")
                register = data[i].Substring(4, 1);

            if (instruction == "jmp" || instruction == "jie" || instruction == "jio")
                offset = int.Parse(Regex.Matches(data[i], @"((\+|-)\d+)").First().Groups[0].Value);

            switch (instruction)
            {
                case "hlf":
                    registers[register] = HLF(registers[register]);
                    break;

                case "tpl":
                    registers[register] = TPL(registers[register]);
                    break;

                case "inc":
                    registers[register] = INC(registers[register]);
                    break;

                case "jmp":
                    i += offset - 1;
                    break;

                case "jie":
                    i += JIE(registers[register]) ? offset - 1 : 0;
                    break;

                case "jio":
                    i += JIO(registers[register]) ? offset - 1 : 0;
                    break;
            }
        }

        Console.WriteLine(registers["b"]);

        registers = new Dictionary<string, long>() { { "a", 1 }, { "b", 0 } };

        for (int i = 0; i < data.Length; i++)
        {
            var instruction = data[i].Substring(0, 3);
            var register = "";
            var offset = 0;

            if (instruction != "jmp")
                register = data[i].Substring(4, 1);

            if (instruction == "jmp" || instruction == "jie" || instruction == "jio")
                offset = int.Parse(Regex.Matches(data[i], @"((\+|-)\d+)").First().Groups[0].Value);

            switch (instruction)
            {
                case "hlf":
                    registers[register] = HLF(registers[register]);
                    break;

                case "tpl":
                    registers[register] = TPL(registers[register]);
                    break;

                case "inc":
                    registers[register] = INC(registers[register]);
                    break;

                //offset - 1 since we will increment the offset at the beginning of the loop
                case "jmp":
                    i += offset - 1;
                    break;

                case "jie":
                    i += JIE(registers[register]) ? offset - 1 : 0;
                    break;

                case "jio":
                    i += JIO(registers[register]) ? offset - 1 : 0;
                    break;

                default:
                    throw new Exception("Invalid instruction");
            }
        }

        Console.WriteLine(registers["b"]);
    }
}