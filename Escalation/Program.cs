using Escalation.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Escalation.Manager;
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
            Earth World = new Earth();
            Random Random = new Random();

            
            /////////////////////////////


            //Creating Managers : 
            IdeologyManager ideologyManager = new IdeologyManager(World, Random);
    


            //build countries : 
            World.initCountries();
            World.setCountriesNeighbors();
         

            

            /////////////////////////////
            ///  - TESTS -  /////////////
            /////////////////////////////  
            for (int i = 0; i < 1000; i++)
            {
                if (i % 10 == 0)
                {
                    //browse each nations in the world with a foreach loop : 
                    foreach (Nation currentNation in World.Nations)
                    {
                        ideologyManager.ManageIdeologies(currentNation.Code);
                    }
                }

                foreach (Nation currentNation in World.Nations)
                {
                    currentNation.DriftIdeologies();
                    //Print majorIdeology in each nation : 
                    Console.WriteLine(currentNation.Code + " : " + currentNation.getIdeologies().Last().Key + " with " + currentNation.getIdeologies().Last().Value);
                }



            }

            


            Console.WriteLine("Finished ! ");
            Console.Read();
        }
    }
}
