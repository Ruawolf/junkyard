using System.IO;

namespace FolderComparer;

internal class Program
{
    static void Main(string[] args)
    {
        var dir1 = args[0];
        var dir2 = args[1];

        if (!DirectorysExists(dir1, dir2))
        {
            return;
        }

        static string NormalizePath(string path) => new DirectoryInfo(path).FullName;
        dir1 = NormalizePath(dir1);
        dir2 = NormalizePath(dir2);

        static string[] GetFiles(string dirPath) =>
            Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories)
            .Select(filePath => Path.GetRelativePath(dirPath, filePath))
            .ToArray();

        Console.WriteLine("処理中です…");
        var filePaths1 = GetFiles(dir1);
        var filePaths2 = GetFiles(dir2);

        var dir1Only = filePaths1.Except(filePaths2).ToArray();
        var dir2Only = filePaths2.Except(filePaths1).ToArray();
        var Intersect = filePaths1.Intersect(filePaths2).ToArray();
        Intersect = FileComparer(dir1, dir2, Intersect);

        static void PrintSection(string title, string[] filePaths)
        {
            if (filePaths.Length > 0)
            {
                Console.WriteLine(title);
                PrintFiles(filePaths);
            }
        }

        PrintSection($"\"{dir1}\"にのみに存在", dir1Only);
        PrintSection($"\"{dir2}\"にのみに存在", dir2Only);
        PrintSection("内容が異なるファイル", Intersect);
    }

    private static void PrintFiles(params string[] filePaths)
    {
        Console.WriteLine();

        foreach (var path in filePaths)
        {
            Console.WriteLine(path);
        }

        Console.WriteLine("================================================================\n");
    }


    private static bool DirectorysExists(params string[] dirPaths)
    {
        foreach (var dirPath in dirPaths)
        {
            if (!Directory.Exists(dirPath))
            {
                Console.WriteLine($"ディレクトリ\"{dirPath}\"が存在しません。");
                return false;
            }
        }

        return true;
    }

    private static string[] FileComparer(string dir1, string dir2, string[] filePaths)
    {
        List<string> list = [];
        //*
        foreach (var filePath in filePaths)
        {
            if (!FileCompare(Path.Join(dir1, filePath), Path.Join(dir2, filePath)))
            {
                list.Add(filePath);
            }
        }
        //*/
        /*
        Parallel.ForEach(filePaths, filePath =>
        {
            if (FileCompare(Path.Join(dir1, filePath), Path.Join(dir2, filePath)))
            {
                lock (list)
                {
                    list.Add(filePath);
                }
            }
        });
        //*/
        return list.ToArray();
    }

    private static bool FileCompare(string filePath1, string filePath2)
    {
        if (filePath1 == filePath2) return true;

        using var fs1 = new FileStream(filePath1, FileMode.Open);
        using var fs2 = new FileStream(filePath2, FileMode.Open);

        if (fs1.Length != fs2.Length) return false;


        int buf1;
        int buf2;

        do
        {
            buf1 = fs1.ReadByte();
            buf2 = fs2.ReadByte();
        }
        while ((buf1 == buf2) && (buf1 != -1));

        return (buf1 - buf2) == 0;
    }
}

