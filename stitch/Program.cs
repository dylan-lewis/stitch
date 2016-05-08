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
            var message = "File stitching cancelled.";


            ////strip paths
            //foreach (var filePath in inputFilePaths)
            //{
            //    int split = filePath.LastIndexOf(Path.DirectorySeparatorChar);
            //    inputFileNames.Add(filePath.Substring(split + 1));
            //}

            Console.WriteLine($@"Working in directory {Dir}");

            var filePaths = Directory.GetFiles(Dir, "*"+ FileType);
            var fileNames = new List<string>();
            try
            {
                var fileCount = filePaths.Length;
                var data = new List<string>();

                Console.WriteLine($"{Cr}Processing {filePaths.Length} file{PrintSIfPlural(filePaths.Length)}:");

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
                    else if (header != content[0])
                    {
                        throw new InvalidDataException("Inconsistant data headings");
                    }
                    else
                    {
                        content.RemoveAt(0); //as already exists
                    }

                    //TODO check for duplicates?
                    Console.Write($"{content.Count} row{PrintSIfPlural(content.Count)}.\t");
                    data.AddRange(content);
                    Console.WriteLine("OK.");

                }

                Console.WriteLine($"{Cr}Writing 1 header row and {data.Count} data row{PrintSIfPlural(data.Count)}.");
                Console.WriteLine($"Output to {WriteFile(data, DetermineOutputFileName(filePaths))}");

                message = "Success.";
            }


            catch (Exception ex)
            {
                Console.WriteLine($"{Cr}\t{ex.Message}. {Cr}Please check input data file before trying again.{Cr}");
            }
            finally
            {
                Console.WriteLine(message);
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
            var outPath = outFolder + Path.DirectorySeparatorChar + 
                        fileName + timeStamp + FileType;

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



        //TODO
        private static string DetermineOutputFileName(string[] inputFiles)
        {
            var outputFileName = "stitched";
            var tempName = String.Empty;
            var inputFileNames = new List<string>();

            foreach (var fileName in inputFiles)
            {
                Console.WriteLine(fileName);
                if (tempName.Equals((String.Empty)))
                {
                    tempName = fileName;
                }
                else
                {
                    //keep chars in common with temp file

                }
            }



            

            return outputFileName;
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
        {
            
        }
    }
}
