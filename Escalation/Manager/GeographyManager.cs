using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Escalation.World;
using Random = Escalation.Utils.Random;

namespace Escalation.Manager
{
    internal class GeographyManager : Manager
    {
        public GeographyManager(Earth World, Random Random) : base(World, Random)
        {
        }



        //Method to initialize the neighbors of each nation, by reading an ajdacency matrix :
        public void initializeNeighbors()
        {

        }
    }
}
