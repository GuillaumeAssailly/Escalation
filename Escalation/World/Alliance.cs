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

        public static List<string> Colors = new()
        {
            "DarkBlue" , "DarkRed",
            "Red", "Blue", "Green", "Yellow", "Orange",
            "Purple", "Pink", "Brown", "Black", "White", "Cyan", "Magenta", "Aqua",
            "AntiqueWhite", "Aquamarine", "Azure", "Beige", "Bisque", "BlanchedAlmond", "BlueViolet", "BurlyWood",
            "CadetBlue", "Chartreuse", "Chocolate", "Coral", "CornflowerBlue", "Cornsilk", "Crimson", "Cyan",
           "DarkCyan", "DarkGoldenrod", "DarkGray", "DarkGreen", "DarkKhaki", "DarkMagenta",
        };
        

        public Alliance(String name)
        {
            cpt++;
            this.id = cpt;
            this.name = name;
            this.color = color;
            this.members = new List<Nation>();
            this.militaryPower = 0; 
            color = Colors[cpt%Colors.Count];
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
