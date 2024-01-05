using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Escalation.World;

namespace Escalation.Utils
{
    internal class Memento
    {
        private Earth state;

        public Memento(Earth state)
        {
            this.state = state;
        }

        public Earth GetState()
        {
            return state;
        }

        public void SetState(Earth state)
        {
            this.state = state;
        }
    }
}
