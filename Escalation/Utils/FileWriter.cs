using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Escalation.World;

namespace Escalation.Utils
{
    public class FileWriter
    {

        //Create all files per Country in a folder :
        public static void CreateFiles(string path, Ecode code)
        {
            if (Directory.Exists(path))
            {
                // Delete existing files
                Directory.Delete(path, true);
            }
           
            //Create folder for country :
            Directory.CreateDirectory(path + code.ToString());
            //Create File for population :
            using (File.Create(path + code.ToString() + "/population.txt")) { };

        }

        public static void AppendLine(string path, string line)
        {
            File.AppendAllText(path,line+Environment.NewLine);
        }
        
    }
}
