using static Core.Helpers;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

var data = await LoadDataAsync(args[0]);

var adventType = typeof(AdventOfCode);
var dayToRun = adventType.GetMethod(args[0], BindingFlags.Static | BindingFlags.Public) ?? throw new ArgumentException("Invalid day");
dayToRun.Invoke(null, new object[] { data });

public static class AdventOfCode
{
    public static void Day1(string[] data)
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

    public static void Day2(string[] data)
    {
        int[] coords = { 1, 1 };
        Dictionary<string, char> coordMap = new()
        {
            {"0,0", '1'},
            {"1,0", '2'},
            {"2,0", '3'},
            {"0,1", '4'},
            {"1,1", '5'},
            {"2,1", '6'},
            {"0,2", '7'},
            {"1,2", '8'},
            {"2,2", '9'},
        };

        foreach (string instructions in data)
        {
            foreach (char direction in instructions)
            {
                switch (direction)
                {
                    case 'U':
                        coords[1]--;
                        break;
                    case 'D':
                        coords[1]++;
                        break;
                    case 'L':
                        coords[0]--;
                        break;
                    case 'R':
                        coords[0]++;
                        break;
                }

                if (coords[1] < 0)
                {
                    coords[1] = 0;
                }
                else if (coords[1] > 2)
                {
                    coords[1] = 2;
                }
                else if (coords[0] < 0)
                {
                    coords[0] = 0;
                }
                else if (coords[0] > 2)
                {
                    coords[0] = 2;
                }
            }

            Console.Write(coordMap[String.Join(',', coords)]);

        }

        Console.WriteLine();

        coordMap = new()
        {
            {"2,0", '1'},
            {"1,1", '2'},
            {"2,1", '3'},
            {"3,1", '4'},
            {"0,2", '5'},
            {"1,2", '6'},
            {"2,2", '7'},
            {"3,2", '8'},
            {"4,2", '9'},
            {"1,3",'A'},
            {"2,3", 'B'},
            {"3,3", 'C'},
            {"2,4", 'D'},
        };

        string[] upCheck = {
            "2,0",
            "1,1",
            "3,1",
            "0,2",
            "4,2"
        };

        string[] downCheck = {
            "0,2",
            "4,2",
            "1,3",
            "3,3",
            "2,4"
        };

        string[] leftCheck = {
            "2,0",
            "1,1",
            "0,2",
            "1,3",
            "2,4"
        };

        string[] rightCheck = {
            "2,0",
            "3,1",
            "4,2",
            "3,3",
            "2,4"
        };

        coords = new int[] { 0, 2 };

        foreach (string instructions in data)
        {
            foreach (char direction in instructions)
            {
                var coordString = String.Join(',', coords);

                switch (direction)
                {
                    case 'U':
                        if (upCheck.Contains(coordString))
                            break;
                        coords[1]--;
                        break;
                    case 'D':
                        if (downCheck.Contains(coordString))
                            break;
                        coords[1]++;
                        break;
                    case 'L':
                        if (leftCheck.Contains(coordString))
                            break;
                        coords[0]--;
                        break;
                    case 'R':
                        if (rightCheck.Contains(coordString))
                            break;
                        coords[0]++;
                        break;
                }
            }

            Console.Write(coordMap[String.Join(',', coords)]);
        }

        Console.WriteLine();
    }

    public static void Day3(string[] data)
    {
        int countPossible = data.Aggregate(0, (acc, line) =>
        {
            int[] sides = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToArray();

            if (sides[0] + sides[1] > sides[2]
            && sides[1] + sides[2] > sides[0]
            && sides[0] + sides[2] > sides[1])
                acc++;

            return acc;
        });

        Console.WriteLine(countPossible);

        countPossible = data.Chunk(3).Aggregate(0, (acc, chunck) =>
        {
            var line1 = chunck[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToArray();
            var line2 = chunck[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToArray();
            var line3 = chunck[2].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToArray();

            //Triage 1
            if (line1[0] + line2[0] > line3[0]
            && line1[0] + line3[0] > line2[0]
            && line2[0] + line3[0] > line1[0])
                acc++;

            //Triage 2
            if (line1[1] + line2[1] > line3[1]
            && line1[1] + line3[1] > line2[1]
            && line2[1] + line3[1] > line1[1])
                acc++;

            //Triage 3
            if (line1[2] + line2[2] > line3[2]
            && line1[2] + line3[2] > line2[2]
            && line2[2] + line3[2] > line1[2])
                acc++;

            return acc;
        });

        Console.WriteLine(countPossible);
    }

    public static void Day4(string[] data)
    {
        Dictionary<string, int> validRooms = new();

        int sectorSum = data.Aggregate(0, (acc, line) =>
        {
            string roomName = line[..line.LastIndexOf('-')];
            int sectorId = int.Parse(line[(line.LastIndexOf('-') + 1)..line.IndexOf('[')]);
            string checksum = line[(line.IndexOf('[') + 1)..line.LastIndexOf(']')];

            var letterGroups = roomName.GroupBy(character => character)
            .Select(group => new
            {
                Count = group.Count(),
                group.Key
            })
            .Where(group => group.Key != '-')
            .OrderByDescending(group => group.Count)
            .ThenBy(group => group.Key)
            .Take(5);

            if (checksum == String.Join("", letterGroups.Select(group => group.Key)))
            {
                acc += sectorId;
                validRooms.Add(roomName, sectorId);
            }

            return acc;
        });

        Console.WriteLine(sectorSum);

        foreach (KeyValuePair<string, int> room in validRooms)
        {
            StringBuilder builder = new();

            foreach (char letter in room.Key)
            {
                if (letter == '-')
                {
                    builder.Append(' ');
                }
                else
                {
                    int move = room.Value % 26;

                    char finalLetter = (char)(letter + move);

                    if (letter + move > 122)
                        finalLetter = (char)((letter + move) - 26);

                    builder.Append(finalLetter);
                }
            }

            if(builder.ToString().Contains("north"))
            {
                Console.WriteLine($"{builder} - {room.Value}");
                return;
            }
        }
    }
}
