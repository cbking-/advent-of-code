namespace Advent2016;

public static class Day4
{
    public static void Run(string[] data)
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

            if (builder.ToString().Contains("north"))
            {
                Console.WriteLine($"{builder} - {room.Value}");
                return;
            }
        }
    }
}