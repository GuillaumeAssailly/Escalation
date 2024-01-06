using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Escalation.World
{

    public enum EventType
    {
        Demographic, Political, Economic, History, Military, Cultural, Environmental, Other,
    }

    public class Event
    {

        [JsonIgnore]
        public EventType Type { get; set; }


        private readonly string description;

        [JsonPropertyName("D")]
        public string Description
        {
            get => description;
            set {  }
        }


        public Event(string description)
        {
           this.Type = Type;
           this.description = description;
        }

    }
}
