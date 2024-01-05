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

       
        //Constructor : 
        protected Manager(Earth World)
        {
            this.World = World;
        }

    }
}
