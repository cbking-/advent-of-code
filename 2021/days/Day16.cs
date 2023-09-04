namespace Advent2021;

public class Packet
{
    private string Payload = string.Empty;
    public int Version = 0;
    private int TypeId = 0;
    private int LengthTypeId = 0;
    public long Value = 0;
    private List<Packet> SubPackets = new List<Packet>();
    public int BitLength = 6;

    public Packet(string _payload)
    {
        Payload = _payload;

        var offset = 0;

        Version = Convert.ToInt32(Payload.Substring(offset, 3), 2);
        offset += 3;

        TypeId = Convert.ToInt32(Payload.Substring(offset, 3), 2);
        offset += 3;

        LengthTypeId = Convert.ToInt32(Payload.Substring(offset, 1), 2);

        if (TypeId != 4)
        {
            if (LengthTypeId == 0)
            {
                offset += 1;

                var subPacketLength = Convert.ToInt32(Payload.Substring(offset, 15), 2);
                offset += 15;

                var parsedLength = 0;

                while (parsedLength < subPacketLength)
                {
                    var subPacket = new Packet(Payload[offset..]);
                    offset += subPacket.BitLength;
                    parsedLength += subPacket.BitLength;

                    SubPackets.Add(subPacket);
                }

                BitLength += 16 + SubPackets.Sum(packet => packet.BitLength);
            }
            else if (LengthTypeId == 1)
            {
                offset += 1;

                var numberOfSubPackets = Convert.ToInt32(Payload.Substring(offset, 11), 2);
                offset += 11;

                while (SubPackets.Count < numberOfSubPackets)
                {
                    var subPacket = new Packet(Payload[offset..]);
                    offset += subPacket.BitLength;

                    SubPackets.Add(subPacket);
                }

                BitLength += 12 + SubPackets.Sum(packet => packet.BitLength);
            }
        }

        else
            Value = ParseLiteralValue(Payload.Substring(offset));

        Value = Evaluate();
    }

    public int GetVersionSum()
    {
        if (SubPackets.Count == 0)
            return Version;

        return Version + SubPackets.Sum(packet => packet.GetVersionSum());
    }

    public long Evaluate()
    {
        switch (TypeId)
        {
            case 0:
                return SubPackets.Sum(packet => packet.Value);
            case 1:
                return SubPackets.Aggregate((long)1, (total, item) => total * item.Value);
            case 2:
                return SubPackets.Min(packet => packet.Value);
            case 3:
                return SubPackets.Max(packet => packet.Value);
            case 4:
                return Value;
            case 5:
                return SubPackets.First().Value > SubPackets.Skip(1).First().Value ? 1 : 0;
            case 6:
                return SubPackets.First().Value < SubPackets.Skip(1).First().Value ? 1 : 0;
            case 7:
                return SubPackets.First().Value == SubPackets.Skip(1).First().Value ? 1 : 0;
            default:
                return 0;
        }
    }

    private long ParseLiteralValue(string message)
    {
        var literalValue = "";

        //batches in max of 5
        // last batch should return less than 5 if padded
        foreach (var number in message.Batch(5))
        {
            BitLength += 5;

            var binary = string.Join("", number);

            literalValue += binary.Substring(1);

            //last group
            if (number.First() == '0')
            {
                break;
            }
        }

        return Convert.ToInt64(literalValue, 2);
    }
}

public static class Day16
{
    public static void Run(string[] data)
    {
        var packet = data.First();

        var map = new Dictionary<char, string> {
                {'0' , "0000"},
                {'1' , "0001"},
                {'2' , "0010"},
                {'3' , "0011"},
                {'4' , "0100"},
                {'5' , "0101"},
                {'6' , "0110"},
                {'7' , "0111"},
                {'8' , "1000"},
                {'9' , "1001"},
                {'A' , "1010"},
                {'B' , "1011"},
                {'C' , "1100"},
                {'D' , "1101"},
                {'E' , "1110"},
                {'F' , "1111"}
            };

        var binaryPacket = string.Join("", packet.Select(character => map[character]));

        var decoder = new Packet(binaryPacket);

        Console.WriteLine($"Part 1: \x1b[93m{decoder.GetVersionSum()}\x1b[0m");
        Console.WriteLine($"Part 2: \x1b[93m{decoder.Value}\x1b[0m");
    }

}