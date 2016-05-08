using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;

namespace stitch
{
    public class Program
    {
        private const string FileType = ".csv";
        private static readonly string Cr = Environment.NewLine;
        private static readonly string Dir = Directory.GetCurrentDirectory();

        public static void Main(string[] args)
        {
            string header = null;
            var filePaths = Directory.GetFiles(Dir, "*"+ FileType);
            var fileNames = new List<string>();
            try
            {
                var fileCount = filePaths.Length;
                var data = new List<string>();

                Console.WriteLine($"{Cr}Processing {filePaths.Length} file{PrintSIfPlural(filePaths.Length)} in {Dir}:");

                foreach (var filePath in filePaths)
                {
                    //remove path data, add to fileName list, and display file name
                    var fileName = GetFileNameFromPath(filePath);
                    fileNames.Add((fileName));
                    Console.Write($"\t{fileName}... ");

                    //read file, ensure data exists
                    var content = File.ReadAllLines(filePath).ToList();
                    if (content.Count == 0)
                        throw new InvalidDataException("No data in file");

                    //check header consistency then add lines to data
                    if (header == null)
                    {
                        header = content[0];
                    }
                    else if (!header.Equals(content[0]))
                    {
                        throw new InvalidDataException("Inconsistant data headings");
                    }
                    else
                    {
                        content.RemoveAt(0); //as data headings already exists
                    }

                    //TODO check for duplicates lines?
                    Console.Write($"{content.Count} row{PrintSIfPlural(content.Count)}\t");
                    data.AddRange(content);
                    Console.WriteLine("OK");

                }

                Console.Write($"{Cr}Output (1 header row and {data.Count} data row{PrintSIfPlural(data.Count)}) to file: ");
                Console.WriteLine($"{WriteFile(data, DetermineOutputFileName(fileNames))}");
                Console.WriteLine("Success.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Cr}\t{ex.Message}. {Cr}Please check input data file before trying again{Cr}File stitching cancelled");
                Console.ReadKey();
            }
        }


        private static string WriteFile(List<string> lines, string fileName)
        {
            var outFolder = Environment.GetFolderPath(
                Environment.SpecialFolder.Desktop
                ) 
                + Path.DirectorySeparatorChar;
            var timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
            var outPath = outFolder + fileName + timeStamp + FileType;

            if (!File.Exists(outPath))
            {
                File.AppendAllLines(outPath, lines.ToArray());
            }

            return outPath;
        }

        private static string GetFileNameFromPath(string filePath)
        {
            var split = filePath.LastIndexOf(Path.DirectorySeparatorChar);
            return filePath.Substring(split + 1);
        }


        private static string DetermineOutputFileName(IEnumerable<string> fileNames)
        {
            const string defaultName = "stitched_";
            var newName = string.Empty;

            foreach (var fileName in fileNames)
            {

                if (newName.Equals((string.Empty)))
                {
                    newName = fileName;
                }
                else if (!fileName.StartsWith(newName))
                {
                    //determine minimum string length; iterate and break on difference
                    var count = newName.Length < fileName.Length ? newName.Length : fileName.Length;
                    for (var i = 0; i < count; i++)
                    {
                        if (!string.Equals(newName[i], fileName[i]))
                        {
                            newName = newName.Substring(0, i);
                            break;
                        }
                    }
                }
            }

            return newName.Length > 0 ? newName + "_" : defaultName;
        }

        private static string PrintSIfPlural(int count)
        {
            return count == 1 ? string.Empty : "s";
        }
    }


    public class InvalidDataException : Exception
    {
        public InvalidDataException(string message)
            : base(message)
        {}
    }
}
