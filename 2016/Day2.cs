public static class Day2{
    public static void Run(string[] data){
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
}