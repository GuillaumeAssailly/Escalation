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

        public EventManager(Earth World, Random Random) : base(World, Random)
        {
            Events = new List<Event>
            {
                new Event(0.1, EventType.Demographic, "Population Growth", "The population of the nation has grown."),
                new Event(0.1, EventType.Demographic, "Population Decline", "The population of the nation has declined."),
                new Event(0.1, EventType.Demographic, "Population Boom", "The population of the nation has boomed."),
                new Event(0.1, EventType.Demographic, "Population Bust", "The population of the nation has busted."),
                new Event(0.1, EventType.Demographic, "Population Stagnation", "The population of the nation has stagnated."),                
                new Event(0.1, EventType.Demographic, "Population Explosion", "The population of the nation has exploded."),
                new Event(0.1, EventType.Demographic, "Population Implosion", "The population of the nation has imploded."),
                new Event(0.1, EventType.Demographic, "Population Growth", "The population of the nation has grown."),
            };
        }
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
    }
}
