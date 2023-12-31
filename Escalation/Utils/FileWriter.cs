﻿using System;
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

        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static void DeleteDir(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path,true);
            }
        }

        public static void AppendLine(string path, string line)
        {
            File.AppendAllText(path,line+Environment.NewLine);
        }

        public static void SaveIdeologies(string path, Dictionary<Ideology, double> ideologies)
        {
            int i = 0;
            foreach (var ideology in ideologies)
            {
                File.AppendAllText(path, ideology.Value.ToString() +";");
                i++;
                if (i % 7 == 0)
                {
                    File.AppendAllText(path, Environment.NewLine);
                }
            }
        }

    }
}
