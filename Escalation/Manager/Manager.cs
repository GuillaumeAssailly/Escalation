using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Escalation.World;

namespace Escalation.Manager
{
    public abstract class Manager
    {
        public Earth World;

        //Attribute of random generator
        public Utils.Random Random;

     


        //Constructor : 
        protected Manager(Earth World, Utils.Random Random)
        {
            this.World = World;
            this.Random = Random;
        }

    }
}
