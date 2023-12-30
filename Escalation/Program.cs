using Escalation.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Escalation.Manager;
using Escalation.World;
using Random = Escalation.Utils.Random;
using System.Threading;

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

            World.Nations.AddRange(FileReader.ReadNationsFromCsv("../../../ESCALATION.csv"));

            foreach (Nation nation in World.Nations)
            {
                nation.initInternalStatistics(
                    (int)Random.NextDouble(), Random.NextDouble(), Random.NextDouble(), Random.NextDouble(),
                    Random.NextDouble(), Random.NextDouble(), Random.NextDouble(), Random.NextDouble());
                nation.initEconomicStats(0, 0, Random.Next(0, 100000));
            }

            geographyManager.initializeNeighbors();
            relationManager.initRelations();


            /////////////////////////////
            /// - FILES CREATION -  /////
            /////////////////////////////
         
            foreach (Nation n in World.Nations)
            {
                FileWriter.CreateFiles("", n.Code);
                FileWriter.SaveIdeologies("idee.txt", n.getIdeologies());
            }

            //print the whole adjacency matrix :
            /*
            for (int i = 0; i < World.Nations.Count; i++)
            {
                for (int j = 0; j < World.Nations.Count; j++)
                {
                    Console.Write(World.RelationsMatrix[i, j] + " ");
                }
                Console.WriteLine();
            }
            */
          

            /////////////////////////////
            ///  - RUNNING -  ///////////
            /////////////////////////////  
            for (int i = 0; i < 100000; i++)
            {
                if (i % 365 == 0)
                {
                    Console.WriteLine("Year : " + World.CurrentDate.Year + " World Tension : " + World.WorldTension);
                }
                //DAY LOOP OVER HERE :
                foreach (Nation currentNation in World.Nations)
                {
                    if (World.CurrentDate.Day == 1)
                    {
                        economyManager.ManageEconomy(currentNation.Code);

                    }

                    //print the 20 richest countries : order by treasury :
                    // List<Nation> richestCountries = World.Nations.OrderByDescending(o => o.Treasury).ToList();

                    if (i % 10 == 0)
                    {
                        ideologyManager.ManageIdeologies(currentNation.Code);
                    }

                    populationManager.ManagePopulation(currentNation.Code);
                    currentNation.DriftIdeologies();
                    //Print majorIdeology in each nation : 
                   // Console.WriteLine(currentNation.Code + " : " + currentNation.getIdeologies().Last().Key + " with " +currentNation.getIdeologies().Last().Value);
                    currentNation.takeAction();

                    
                }
                relationManager.GoToWar();
                relationManager.ManageWars();

                World.AddDay();
            }







        }
    }
}