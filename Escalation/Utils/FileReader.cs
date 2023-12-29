using Escalation.World;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escalation.Utils
{
    public class FileReader
    {
        //method to read population file and return list of decimal
        public static List<decimal> ReadPopulation(string path)
        {
            List<decimal> population = new List<decimal>();
            string[] lines = System.IO.File.ReadAllLines(path);
            foreach (string line in lines)
            {
                population.Add(decimal.Parse(line));
            }

            return population;
        }

        public static char[,] ReadMatrixFromFile(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);

                int rows = lines.Length;
                int cols = lines[0].Length;

                char[,] matrix = new char[rows, cols];

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        matrix[i, j] = lines[i][j];
                    }
                }

                return matrix;
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., file not found, format issues)
                Console.WriteLine($"Error reading matrix from file: {ex.Message}");
                return null;
            }
        }


        public static List<Nation> ReadNationsFromCsv(string filePath)
        {
            List<Nation> nations = new List<Nation>();

            using (var reader = new StreamReader(filePath))
            {
                Ecode index = 0;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(';');
                    if (parts.Length == 12)
                    {


                        var nation = new Nation
                        (
                            index,
                            0,
                            10,
                            int.Parse(parts[1], CultureInfo.InvariantCulture),
                            double.Parse(parts[2], CultureInfo.InvariantCulture),
                            double.Parse(parts[3], CultureInfo.InvariantCulture),
                            decimal.Parse(parts[4], CultureInfo.InvariantCulture),
                            double.Parse(parts[5], CultureInfo.InvariantCulture),
                            double.Parse(parts[6], CultureInfo.InvariantCulture),
                            double.Parse(parts[7], CultureInfo.InvariantCulture),
                            int.Parse(parts[8], CultureInfo.InvariantCulture),
                            int.Parse(parts[9], CultureInfo.InvariantCulture),
                            int.Parse(parts[10], CultureInfo.InvariantCulture),
                            decimal.Parse(parts[11], CultureInfo.InvariantCulture)
                        );
                        nations.Add(nation);
                    }

                    index++;
                }
            }

            return nations;
        }
    }

}
