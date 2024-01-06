using Escalation.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Escalation.Manager;
using Escalation.World;
using Random = Escalation.Utils.Random;
using System.Threading;
using System.Text.Json;
using System.Globalization;

namespace Escalation
{



    internal class Program
    {

        public static Earth World { get; private set; }
       
        public static UInt64 Seed_64 = 10465520;
        public static UInt32 Mat1_64 = 0xfa051f40;
        public static UInt32 Mat2_64 = 0xffd0fff4;
        public static UInt64 TMat_64 = 0x58d02ffeffbfffbc;

     

        static void Main(string[] args)
        {
            World = Earth.getWorld();
            Random.SetSeed(Seed_64);

            /////////////////////////////


            //Creating Managers : 
            /*
            ideologyManager = new IdeologyManager(World);
            populationManager = new PopulationManager(World);
            economyManager = new EconomyManager(World);
            relationManager = new RelationManager(World);
            geographyManager = new GeographyManager(World);
            */

            //Using a Facade Design Pattern, we now have : 
            ManagerFront managerFront = new ManagerFront(World);


            //Creating Serializer/Saver : 
            EarthSaver earthSaver = new JsonEarthSaver();

            //Creating Countries from CSV :
            World.Nations.AddRange(FileReader.ReadNationsFromCsv("ESCALATION.csv"));


            //We initialize the internal stats of nations with random values :
            foreach (Nation nation in World.Nations)
            {
                nation.initInternalStatistics(
                    Random.NextDouble(), Random.NextDouble(), Random.NextDouble(), Random.NextDouble(),
                    Random.NextDouble(), Random.NextDouble(), Random.NextDouble());
                nation.initEconomicStats(0, 0, Random.Next(0, 100000));
            }


            /*
            geographyManager.initializeNeighbors("../../../neighbors.txt");
            relationManager.initAlliances();
            relationManager.initRelations();
            */

            managerFront.InitAll("../../../neighbors.txt");


            /////////////////////////////
            /// - Files -  //////////////
            ///
            FileWriter.DeleteDir("History");
          
            Directory.CreateDirectory("History");
            

            /////////////////////////////
            ///  - TESTS -  /////////////
            /////////////////////////////  
            for (int i = 0; i < 30000; i++)
            {
                Memento memento = new Memento(World);
                earthSaver.SaveState(memento);
                earthSaver.SaveLastAndRemove("History\\" + World.CurrentDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + ".json");
                
                //FileWriter.AppendLine("History\\"+World.CurrentDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + ".json", JsonSerializer.Serialize(World));
                
                

                //DAY LOOP OVER HERE :
                foreach (Nation currentNation in World.Nations)
                {

                    if (i % 10 == 0)
                    {
                        managerFront.ManageIdeologies(currentNation.Code);
                    }


                    if (World.CurrentDate.Day == 1)
                    {
                        managerFront.ManageEconomy(currentNation.Code);
                        managerFront.updateRelations(currentNation);
                        currentNation.DriftIdeologies();
                    }



                    //print the 20 richest countries : order by treasury :
                    // List<Nation> richestCountries = World.Nations.OrderByDescending(o => o.Treasury).ToList();



                    managerFront.ManagePopulation(currentNation.Code);


                    //Print majorIdeology in each nation : 
                    //Console.WriteLine(currentNation.Code + " : " + currentNation.getIdeologies().Last().Key + " with " + currentNation.getIdeologies().Last().Value);
                    currentNation.takeAction();
                }

                /*
                relationManager.GoToWar();
                relationManager.ManageAlliances();
                relationManager.ManageWars();
                relationManager.ManageTension();
                World.AddDay();
                */

                managerFront.EndDay();

            }
        }
    }
}