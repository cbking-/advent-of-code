namespace Advent2016;

public static class Day1
{
    public static void Run(string[] data)
    {
        string[] input = data[0].Split(',', StringSplitOptions.TrimEntries);
        int currentDirectionIndex = 0;
        char[] directions = { 'n', 'e', 's', 'w' };
        Dictionary<char, int> accumulator = new()
        {
            {'n', 0},
            {'e', 0},
            {'s', 0},
            {'w', 0}
        };

        foreach (string instruction in input)
        {
            if (instruction.StartsWith('L'))
            {
                currentDirectionIndex--;
            }
            else
            {
                currentDirectionIndex++;
            }

            if (currentDirectionIndex == 4)
            {
                currentDirectionIndex = 0;
            }
            else if (currentDirectionIndex == -1)
            {
                currentDirectionIndex = 3;
            }

            accumulator[directions[currentDirectionIndex]] += int.Parse(instruction[1..]);
        }

        Console.WriteLine(Math.Abs(accumulator['n'] - accumulator['s']) + Math.Abs(accumulator['e'] - accumulator['w']));


        HashSet<string> coords = new() { "0,0" };
        int[] currentCoords = { 0, 0 };

        foreach (string instruction in input)
        {
            if (instruction.StartsWith('L'))
            {
                currentDirectionIndex--;
            }
            else
            {
                currentDirectionIndex++;
            }

            if (currentDirectionIndex == 4)
            {
                currentDirectionIndex = 0;
            }
            else if (currentDirectionIndex == -1)
            {
                currentDirectionIndex = 3;
            }

            bool breakLoop = false;

            foreach (int _ in Enumerable.Range(0, int.Parse(instruction[1..])))
            {
                switch (currentDirectionIndex)
                {
                    case 0:
                        currentCoords[0]++;
                        break;

                    case 1:
                        currentCoords[1]++;
                        break;

                    case 2:
                        currentCoords[0]--;
                        break;

                    case 3:
                        currentCoords[1]--;
                        break;

                    default:
                        break;
                }

                if (coords.Contains(String.Join(",", currentCoords)))
                {
                    breakLoop = true;
                    break;
                }
                else
                {
                    coords.Add(String.Join(",", currentCoords));
                }
            }

            if (breakLoop)
                break;
        }

        Console.WriteLine(String.Join(",", currentCoords));
    }
}