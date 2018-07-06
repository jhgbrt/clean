using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Console;
using static System.IO.Directory;
using static System.IO.SearchOption;

namespace clean
{
    static class Program
    {
        static void Main()
        {
            var cd = new DirectoryInfo(GetCurrentDirectory());
            cd.DeleteDirectories("bin", "obj", "testresults", "packages");
            cd.DeleteFiles("*.suo", "*.vssscc", "*.user");
        }

        static void DeleteDirectories(this DirectoryInfo dir, params string[] folderNames)
            => dir.FindDirectories(folderNames).ForEach(d => d.Delete(true), d => d.DeleteFiles("*"));

        static void DeleteFiles(this DirectoryInfo dir, params string[] searchPatterns)
            => dir.FindFiles(searchPatterns).ForEach(f => f.Delete());

        static IEnumerable<DirectoryInfo> FindDirectories(this DirectoryInfo dir, params string[] folderNames)
            => dir.EnumerateDirectories("*", AllDirectories).Where(d => folderNames.Contains(d.Name.ToLowerInvariant()));

        static IEnumerable<FileInfo> FindFiles(this DirectoryInfo d, params string[] searchPatterns)
            => searchPatterns.SelectMany(pattern => d.EnumerateFiles(pattern, AllDirectories));

        static void ForEach<T>(this IEnumerable<T> fileSystemInfos, Action<T> delete, Action<T> onException = null) where T: FileSystemInfo
        {
            foreach (var f in fileSystemInfos)
            {
                try
                {
                    WriteLine(f.FullName);
                    delete(f);
                }
                catch (Exception e)
                {
                    LogError(f,e);
                    onException?.Invoke(f);
                }
            }
        }

        private static void LogError(FileSystemInfo f, Exception e)
        {
            try
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine($"Error deleting '{f.FullName}': {e.Message}");
            }
            finally
            {
                ResetColor();
            }
        }
    }
}
