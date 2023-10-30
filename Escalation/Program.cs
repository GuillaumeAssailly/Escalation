using Escalation.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Escalation.World;
using Random = Escalation.Utils.Random;

namespace Escalation
{
    internal class Program
    {


        public static UInt64 Seed_64 = 1;
        public static UInt32 Mat1_64 = 0xfa051f40;
        public static UInt32 Mat2_64 = 0xffd0fff4;
        public static UInt64 TMat_64 = 0x58d02ffeffbfffbc;

        static void Main(string[] args)
        {



            Nation France = new Nation(Ecode.FRA, 0.5, 100, 0.01, 0.0, 0.2, 0.3, 0.48, 0.0, 0.01);


            France.SetRisingIdeology(Ideology.Despotism, 0.001);
            France.DriftIdeologies();

            France.SetRisingIdeology(Ideology.Communism, 0.001);
            France.DriftIdeologies();

            France.SetRisingIdeology(Ideology.Fascism, 0.1);
            France.DriftIdeologies();

            France.SetRisingIdeology(Ideology.Fascism, 0.04);
            for (int i = 0; i < 40; i++)
            {

                France.DriftIdeologies();
            }

            France.SetRisingIdeology(Ideology.Despotism, 0.001);

            for (int i = 0; i < 5; i++)
            {
                France.DriftIdeologies();

            }



            Console.Read();
        }
    }
}
