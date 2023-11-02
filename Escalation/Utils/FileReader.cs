using System;
using System.Collections.Generic;
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
    }
}
