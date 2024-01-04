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
        public static Random Random { get; private set; }
        public static IdeologyManager ideologyManager { get; private set; }
        public static PopulationManager populationManager { get; private set; }

        public static EconomyManager economyManager { get; private set; }

        public static GeographyManager geographyManager { get; private set; }

        public static RelationManager relationManager { get; private set; }

        public static UInt64 Seed_64 = 1;
        public static UInt32 Mat1_64 = 0xfa051f40;
        public static UInt32 Mat2_64 = 0xffd0fff4;
        public static UInt64 TMat_64 = 0x58d02ffeffbfffbc;

     

        static void Main(string[] args)
        {
            World = new Earth();
            Random = new Random();

            /////////////////////////////


            //Creating Managers : 
            ideologyManager = new IdeologyManager(World, Random);
            populationManager = new PopulationManager(World, Random);
            economyManager = new EconomyManager(World, Random);
            relationManager = new RelationManager(World, Random);
            geographyManager = new GeographyManager(World, Random);

            //build countries : 
            //World.initCountries();

            World.Nations.AddRange(FileReader.ReadNationsFromCsv("ESCALATION.csv"));


            foreach (Nation nation in World.Nations)
            {
                nation.initInternalStatistics(
                    (int)Random.NextDouble(), Random.NextDouble(), Random.NextDouble(), Random.NextDouble(),
                    Random.NextDouble(), Random.NextDouble(), Random.NextDouble(), Random.NextDouble());
                nation.initEconomicStats(0, 0, Random.Next(0, 100000));
            }

            geographyManager.initializeNeighbors("../../../neighbors.txt");
            relationManager.initAlliances();
            relationManager.initRelations();


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
                FileWriter.AppendLine("History\\"+World.CurrentDate.ToString("yyyy-M-dd", CultureInfo.InvariantCulture) + ".json", JsonSerializer.Serialize(World));
                
                //DAY LOOP OVER HERE :
                foreach (Nation currentNation in World.Nations)
                {
                    if (i % 10 == 0)
                    {
                        ideologyManager.ManageIdeologies(currentNation.Code);
                    }


                    if (World.CurrentDate.Day == 1)
                    {
                        economyManager.ManageEconomy(currentNation.Code);
                        relationManager.updateRelations(currentNation);
                        currentNation.DriftIdeologies();
                    }



                    //print the 20 richest countries : order by treasury :
                    // List<Nation> richestCountries = World.Nations.OrderByDescending(o => o.Treasury).ToList();



                    populationManager.ManagePopulation(currentNation.Code);


                    //Print majorIdeology in each nation : 
                    //Console.WriteLine(currentNation.Code + " : " + currentNation.getIdeologies().Last().Key + " with " + currentNation.getIdeologies().Last().Value);
                    currentNation.takeAction();
                }

                relationManager.ManageAlliances();
                relationManager.GoToWar();
                relationManager.ManageWars();
                relationManager.ManageTension();
                World.AddDay();





              







            }
        }
    }
}