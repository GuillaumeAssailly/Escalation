using Escalation.Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Escalation.World
{
    public class Earth
    {
        //Parameters :
        const int nbNations = 151;


        private static Earth _instance;

        private double worldTension;

        public double WorldTension
        {
            get => worldTension;
            set
            {
                //keep it between 5 and 100 :
                if (value < 5)
                {
                    worldTension = 5;
                }
                else if (value > 100)
                {
                    worldTension = 100;
                }
                else
                {
                    worldTension = value;
                }
            }
        }

        [JsonPropertyName("LCE")]
        public List<Event> CurrentEvents { get; set; }



        public DateTime CurrentDate { get; set; }


        //list of nations : 
        public List<Nation> Nations { get; set; }


        //list of alliances (and military pacts) : 
        public List<Alliance> Alliances { get; set; }


        //list of wars :
        public List<War> Wars { get; set; }
        
        

        //ajdacency matrix of the countries : 
        public char[,] AdjacencyMatrix;

        public int[,] RelationsMatrix;

        public bool[,] WarMatrix;

        //list of all Influence Sphere : 

        private void setAdjacencyMatrixFromFile()
        {
            //TODO : create a specific class to deal with reading files and all of that stuff:

            //print current directory :
            Console.WriteLine("Current directory : " + Directory.GetCurrentDirectory());


            string filePath = "../../adjacencyMatrix.txt";

            if (File.Exists(filePath))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        int rowCount = 200;
                        int colCount = 200;
                        AdjacencyMatrix = new char[rowCount, colCount];
                        /*
                        for (int i = 0; i < rowCount; i++)
                        {
                            string line = reader.ReadLine();
                           
                            string[] values = line.Split(' ');
                          
                            for (int j = 0; j < colCount; j++)
                            {
                                adjacencyMatrix[i, j] = values[j][0]; 
                            }
                        }

                        // Afficher la matrice lue
                        for (int i = 0; i < rowCount; i++)
                        {
                            for (int j = 0; j < colCount; j++)
                            {
                                Console.Write(adjacencyMatrix[i, j] + " ");
                            }
                            Console.WriteLine();
                        }*/
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("Error when opening file :  " + e.Message);
                }
            }
            else
            {
                Console.WriteLine("No adjacencyMatrix file found");
            }


        }




     

        public void AddDay()
        {
            CurrentDate = CurrentDate.AddDays(1);
        }


        private Earth()
        {
            Nations = new List<Nation>();
            Wars = new List<War>();
            Alliances = new List<Alliance>();
            //setAdjacencyMatrixFromFile();
            CurrentDate =  new DateTime(1966, 1, 1);
            WorldTension = 10;
            CurrentEvents = new List<Event>();

        }

        public static Earth getWorld()
        {
            if (_instance == null)
            {
                _instance = new Earth();
            }

            return _instance;
        }

    }
}
