using System;
using System.IO;
using System.Linq;
using static System.Console;
using static System.IO.SearchOption;

namespace clean
{
    class Program
    {
        static void Main(string[] args)
        {
            var enumerable = args.Concat(new [] {"bin", "obj", "TestResults", "*.suo"}).ToLookup(x => x.Contains("*"));
            DeleteDirectories(enumerable[false].ToArray());
            DeleteFiles(new DirectoryInfo("."), enumerable[true].ToArray());
        }

        private static void DeleteDirectories(params string[] folderNames)
        {
            var baseDir = new DirectoryInfo(".");

            var foldersToDelete = from folderName in folderNames
                                  from d in baseDir.EnumerateDirectories(folderName, AllDirectories)
                                  where d.Name.Equals(folderName, StringComparison.InvariantCultureIgnoreCase)
                                  select d;

            foreach (var d in foldersToDelete)
            {
                try
                {
                    WriteLine(d.FullName);
                    d.Delete(true);
                }
                catch (Exception e)
                {
                    LogError($"Error deleting folder '{d.FullName}'", e);
                    DeleteFiles(d, "*");
                }
            }
        }

        private static void DeleteFiles(DirectoryInfo d, params string[] searchPatterns)
        {
            var filesToDelete = 
                from pattern in searchPatterns
                from f in d.EnumerateFiles(pattern, AllDirectories)
                select f;

            foreach (var f in filesToDelete)
            {
                try
                {
                    WriteLine(f.FullName);
                    f.Delete();
                }
                catch (Exception e)
                {
                    LogError($"Error deleting file '{f.FullName}'", e);
                }
            }
        }

        private static void LogError(string error, Exception e)
        {
            var c = ForegroundColor;
            try
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine($"{error}: {e.Message}");
            }
            finally
            {
                ForegroundColor = c;
            }
        }
    }
}
