using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escalation
{
    internal class Program
    {
        static void Main(string[] args) {

            Nation France = new Nation(Ecode.FRA, 0.5, 100,  0.01,  0.0,  0.2,  0.3,  0.48,  0.0,  0.01);


            France.SetRisingIdeology(Ideology.Despotism,  0.001);

            for (int i = 0; i < 20000; i++)
            {
                France.DriftIdeologies();
            }
            
            France.SetRisingIdeology(Ideology.Communism,  0.00001);
            for (int i = 0; i < 50000; i++)
            {
                France.DriftIdeologies();
            }

            France.SetRisingIdeology(Ideology.Fascism,  100);

            for (int i = 0; i < 80000; i++)
            {
                France.DriftIdeologies();
            }

            Console.Read();

        }
    }
}
