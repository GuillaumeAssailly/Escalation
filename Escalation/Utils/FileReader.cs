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
                    if (parts.Length == 5)
                    {


                        var nation = new Nation
                        (
                            index,
                            0,
                            10,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            decimal.Parse(parts[1], CultureInfo.InvariantCulture),
                            double.Parse(parts[2], CultureInfo.InvariantCulture),
                            double.Parse(parts[3], CultureInfo.InvariantCulture),
                            double.Parse(parts[4], CultureInfo.InvariantCulture)
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
