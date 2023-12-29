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

        
        private List<Ecode> members;

        private int militaryPower;


        public Alliance(String name, String color)
        {
            this.name = name;
            this.color = color;
            this.members = new List<Ecode>();
        }



        public void AddMember(Ecode ecode)
        {
            this.members.Add(ecode);
        }

        public void RemoveMember(Ecode ecode)
        {
            this.members.Remove(ecode);
        }




    }
}
