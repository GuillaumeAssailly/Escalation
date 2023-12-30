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

        private String color;

        private static int cpt = -1;

        
        private List<Nation> members;

        private int militaryPower;
        private int id;

        public Alliance(String name, String color)
        {
            cpt++;
            this.id = cpt;
            this.name = name;
            this.color = color;
            this.members = new List<Nation>();
        }



        public void AddMember(Nation n)
        {
            this.members.Add(n);
            n.MilitaryPact = id;
        }

        public void RemoveMember(Nation nation)
        {
            this.members.Remove(nation);
            nation.MilitaryPact = -1;
        }

        public List<Nation> GetMembers()
        {
            return members;
        }
      



    }
}
