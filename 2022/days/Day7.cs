namespace Advent2022;

public class ElfDirectory
{
    public string Name = string.Empty;

    public List<ElfFile> Files = new();

    public List<ElfDirectory>? Directories;

    public int Size = 0;

    public int GetSize()
    {
        if (Size == 0)
            Size = (Directories?.Sum(directory => directory.GetSize()) ?? 0) + Files.Sum(file => file.Size);

        return Size;
    }
}

public class ElfFile
{
    public string Name = string.Empty;
    public int Size = 0;
}


public static class Day7
{
    public static void Run(string[] data)
    {
        Stack<string> path = new();
        List<ElfDirectory> fileSystem = new();
        ElfDirectory currentDir = new();

        foreach (var line in data)
        {
            var tokens = line.Split(' ');

            if (tokens.First() == "$")
            {
                var command = tokens.Skip(1).First();
                var argument = tokens.Skip(2).FirstOrDefault();

                if (command == "cd")
                {
                    if (argument == "..")
                    {
                        path.Pop();
                    }
                    else
                    {
                        path.Push(argument ?? "");

                        currentDir = new ElfDirectory
                        {
                            Name = string.Join('/', path)
                        };
                    }
                }
                else
                {
                    //theres no processing for ls commands
                    continue;
                }
            }
            else
            {
                //Taking advantage of object references
                var existingDir = fileSystem.Find(dir => dir.Name == currentDir.Name);

                if (existingDir is not null)
                {
                    //The directory was found in a previous command
                    // and we are populating that directory now
                    currentDir = existingDir;
                }
                else
                {
                    fileSystem.Add(currentDir);
                }

                if (tokens.First() == "dir")
                {
                    if (currentDir.Directories is null)
                        currentDir.Directories = new();

                    //add to the path temporarily
                    path.Push(tokens.Last());

                    var newDir = new ElfDirectory
                    {
                        Name = string.Join('/', path)
                    };

                    path.Pop();

                    fileSystem.Add(newDir);
                    currentDir.Directories.Add(newDir);
                }
                else
                {
                    currentDir.Files.Add(new ElfFile
                    {
                        Name = tokens.Last(),
                        Size = int.Parse(tokens.First())
                    });
                }
            }
        }

        var sum = fileSystem.Where(dir => dir.GetSize() <= 100000)
                            .Sum(dir => dir.Size);

        Console.WriteLine(sum);

        var freeSpace = 70000000 - fileSystem.First().GetSize();
        var spaceToClear = 30000000 - freeSpace;

        var dirToDelete = fileSystem.Where(dir => dir.Size >= spaceToClear).Min(dir => dir.Size);

        Console.WriteLine(dirToDelete);
    }

}