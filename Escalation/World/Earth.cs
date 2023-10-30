using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escalation.World
{
    public class Earth
    {
        //Parameters :
        const int nbNations = 5;




        public DateTime CurrentDate;


        //list of nations : 
        public List<Nation> Nations;

        


        //ajdacency matrix of the countries : 
        private char[,] adjacencyMatrix;


        //list of all Military Pacts : 


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
                        int rowCount = nbNations;
                        int colCount = nbNations;
                        adjacencyMatrix = new char[rowCount, colCount];

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
                        }
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




        public void setCountriesNeighbors()
        {
            foreach(Nation currentNation in Nations)
            {
                Dictionary<Ecode, char > neighbors = new Dictionary<Ecode, char>();

                //Read the adjacency matrix and set neighbors accordingly : 
                for (int j = 0; j < nbNations; j++)
                {
                    if (adjacencyMatrix[(int)currentNation.Code, j] != 'X')
                    {
                        neighbors.Add((Ecode)j, adjacencyMatrix[(int)currentNation.Code, j]);
                    }
                }

                currentNation.SetNeighbors(neighbors);
            }
        }

        public void initCountries()
        {
            //European Countries : 
            Nations.Add(new Nation(Ecode.FRA, 100, 15, 0.05, 0.10, 0.25, 0.40, 0.10, 0.05, 0.05));
            Nations.Add(new Nation(Ecode.ALL, 100, 15, 0, 0, 0, 0, 0, 0, 1));
            Nations.Add(new Nation(Ecode.ITA, 100, 15, 0, 0, 0.50, 0.45, 0.05, 0, 0));
            Nations.Add(new Nation(Ecode.ROY, 100, 15, 0.05, 0, 0, 0, 0.5, 0.45, 0));
            Nations.Add(new Nation(Ecode.ESP, 100, 15, 0, 0, 0, 0, 0.5, 0.5, 0));

        }

        public void Day()
        {
            CurrentDate = CurrentDate.AddDays(1);
        }


        public Earth()
        {
            Nations = new List<Nation>();
            setAdjacencyMatrixFromFile();
            CurrentDate =  new DateTime(2022, 1, 1);
        }

    }
}
