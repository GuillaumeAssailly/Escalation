using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Escalation.World;
using Random = Escalation.Utils.Random;

namespace Escalation.Manager
{
    internal class EventManager : Manager
    {
        private List<Event> Events;

       
        /*
        public void ManageEvents(Ecode code)
        {
            Nation CurrentNation = World.Nations[(int)code];

            Event e = SelectRandomEvent();

            if (e != null)
            {
                Console.WriteLine(e.Name);
                Console.WriteLine(e.Description);
            }
        }

        
        private Event SelectRandomEvent()
        {
            double totalProba = 1;

            double random = Random.NextDouble();
             
                totalProba -= e.Probability;

                if (random >= totalProba)
                {
                    return e;
                }
           

            return null;
        }*/
        public EventManager(Earth World) : base(World)
        {
        }
    }
}
