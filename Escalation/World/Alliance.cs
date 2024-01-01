using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escalation.World
{

    
    public class Alliance
    {
        private String name;

        public String color;

        private static int cpt = -1;

        
        private List<Nation> members;

        private decimal militaryPower;
        private int id;

        private static List<string>Colors = new() {"Red", "Blue", "Green", "Yellow", "Orange", "Purple", "Pink", "Brown", "Black", "White", "Grey", "Cyan", "Magenta"};
    

        public Alliance(String name, String color)
        {
            cpt++;
            this.id = cpt;
            this.name = name;
            this.color = color;
            this.members = new List<Nation>();
            this.militaryPower = 0; 
        }



        public void AddMember(Nation n)
        {
            this.members.Add(n);
            //sort list by military power
            this.members = this.members.OrderByDescending(n => n.Military).ToList();
            n.MilitaryPact = id;
          
        }

        public void RemoveMember(Nation nation)
        {
            this.members.Remove(nation);
            this.members = this.members.OrderByDescending(n => n.Military).ToList();
            nation.MilitaryPact = -1;
           
        }

        public List<Nation> GetMembers()
        {
            return members;
        }

        public decimal GetMilitaryPower()
        {
            return members.Sum(n => n.Military);
        }


    }
}
