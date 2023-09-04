namespace Advent2015;

public static class Day14
{
    public static void Run(string[] data)
    {
        var winner = 0;
        var raceDuration = 2503;

        foreach (var line in data)
        {
            var pattern = @"(\w+) can fly (\d+) km/s for (\d+) seconds, but then must rest for (\d+) seconds.";
            var match = Regex.Matches(line, pattern).First();

            var reindeer = match.Groups[1].Value;
            var speed = int.Parse(match.Groups[2].Value);
            var speedDuration = int.Parse(match.Groups[3].Value);
            var restDuration = int.Parse(match.Groups[4].Value);

            var distance = speed * speedDuration;
            var totalDuration = speedDuration + restDuration;

            var totalDistance = distance * (raceDuration / totalDuration);
            var speedTimeLeft = raceDuration % totalDuration;

            if (speedTimeLeft >= speedDuration)
            {
                totalDistance += distance;
            }
            else
            {
                totalDistance += speed * speedTimeLeft;
            }

            winner = Math.Max(winner, totalDistance);
        }

        Console.WriteLine($"Part 1: {winner}");

        var race = new Dictionary<string, int[]>();
        var points = new Dictionary<string, int>();

        foreach (var line in data)
        {
            var pattern = @"(\w+) can fly (\d+) km/s for (\d+) seconds, but then must rest for (\d+) seconds.";
            var match = Regex.Matches(line, pattern).First();

            var reindeer = match.Groups[1].Value;
            var speed = int.Parse(match.Groups[2].Value);
            var speedDuration = int.Parse(match.Groups[3].Value);
            var restDuration = int.Parse(match.Groups[4].Value);

            var distance = speed * speedDuration;
            var totalDuration = speedDuration + restDuration;

            race.Add(reindeer, Enumerable.Repeat(0, raceDuration).ToArray());
            points.Add(reindeer, 0);
            var accumulator = 0;

            foreach (var seconds in Enumerable.Range(1, raceDuration))
            {
                var totalDistance = distance * (seconds / totalDuration);
                var speedTimeLeft = seconds % totalDuration;

                if (speedTimeLeft > speedDuration)
                {
                    race[reindeer][seconds - 1] = accumulator;
                }
                else
                {
                    race[reindeer][seconds - 1] = (speed * speedTimeLeft) + totalDistance;
                    accumulator = (speed * speedTimeLeft) + totalDistance;
                }
            }
        }

        foreach (var second in Enumerable.Range(1, raceDuration))
        {
            var momentWinner = race.Max(reindeer => reindeer.Value[second - 1]);
            var scorer = race.First(reindeer => reindeer.Value[second - 1] == momentWinner).Key;
            points[scorer] += 1;
        }

        Console.WriteLine($"Part 2: {points.Max(kvp => kvp.Value)}");
    }

}