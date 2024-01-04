using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Escalation.Utils;
using Escalation.World;
using Random = Escalation.Utils.Random;

namespace Escalation.Manager
{
    public class GeographyManager : Manager
    {
        public GeographyManager(Earth World, Random Random) : base(World, Random)
        {
        }



        //Method to initialize the neighbors of each nation, by reading an ajdacency matrix :
        public void initializeNeighbors(string path)
        {
            World.AdjacencyMatrix = FileReader.ReadMatrixFromFile(path);
            foreach (Nation currentNation in World.Nations)
            {
                Dictionary<Ecode, char> neighbors = new Dictionary<Ecode, char>();

                //Read the adjacency matrix and set neighbors accordingly : 
                for (int j = 0; j < World.Nations.Count; j++)
                {
                    if (World.AdjacencyMatrix[(int)currentNation.Code, j] != 'X')
                    {
                        neighbors.Add((Ecode)j, World.AdjacencyMatrix[(int)currentNation.Code, j]);
                    }
                }

                currentNation.SetNeighbors(neighbors);
            }
        }
    }
}
