using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace stitch
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const string dir = @"C:\temp\";  //Directory.GetCurrentDirectory();
            const string fileType = "*.csv";
            string header = null;

            Console.WriteLine($@"Working in directory {dir}");

            try
            {
                string[] filePaths = Directory.GetFiles(dir, fileType);
                int fileCount = filePaths.Length;

                List<string> data = new List<string>();

                Console.WriteLine($"\nProcessing {fileCount} file" +
                                  ((fileCount != 1) ? "s:" : ":"));

                foreach (string filePath in filePaths)
                {

                    Console.Write($"\t {filePath}... ");

                    //read file and check header consistency
                    List<string> content = File.ReadAllLines(filePath).ToList();
                    if (header == null)
                    {
                        header = content[0];
                    }
                    else if (header != content[0])
                    {
                        const string error = "Inconsistant data headings.";
                        throw new NotSupportedException(error);
                    }
                    else
                    {
                        content.RemoveAt(0); //as already exists
                    }

                    //TODO check for duplicates?

                    data.AddRange(content);
                    Console.WriteLine($" { content.Count} rows.\tOK");

                }

                Console.WriteLine($"Writing header (1 row) and data ({data.Count} " + 
                    ((data.Count == 1) ? "row" : "rows") + ").");

                Console.WriteLine($"Output to {WriteFile(data)}");

                Console.WriteLine("\nSuccess.");
            }


            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + Environment.NewLine + "Processing cancelled.");
            }
            finally
            {
                Console.ReadLine();
            }
        }

        public static string WriteFile(List<string> lines, string fileName = "output")
        {
            string outExtension = ".csv";
            string outFolder = Environment.GetFolderPath(
                Environment.SpecialFolder.Desktop
                );
            string outTimeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string outPath = outFolder + Path.DirectorySeparatorChar + 
                        fileName + outTimeStamp + outExtension;

            if (!File.Exists(outPath))
            {
                File.AppendAllLines(outPath, lines.ToArray());
            }

            return outPath;
        }

    }
}
