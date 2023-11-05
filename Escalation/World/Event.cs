using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escalation.World
{

    public enum EventType
    {
        Demographic, Political, Economic, History, Military, Cultural, Environmental, Other,
    }

    public class Event
    {
        private double probability;
        public double Probability { get; set; }

        private EventType Type;

        private string name;
        public string Name { get; set; }

        private string description;
        public string Description { get; set; }


        public Event(double probability, EventType Type , string name, string description)
        {
           this.probability = probability;
           this.Type = Type;
           this.name = name;
           this.description = description;
        }

    }
}
