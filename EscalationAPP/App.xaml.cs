using System;
using System.Linq;
using System.Threading;
using System.Windows;
using Escalation.Manager;
using Escalation.Utils;
using Escalation.World;
using Random = Escalation.Utils.Random;


namespace EscalationAPP
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>


    public partial class App : Application
    {
        public Earth World { get; private set; }
        public Random Random { get; private set; }
        public IdeologyManager ideologyManager { get; private set; }
        public PopulationManager populationManager { get; private set; }
       
        public EconomyManager economyManager { get; private set; }

        public GeographyManager geographyManager { get; private set; }

        public RelationManager relationManager { get; private set; }

        public App()
        {
            World = Earth.getWorld();
            Random.SetSeed(14564412);
            /////////////////////////////


            //Creating Managers : 
            ideologyManager = new IdeologyManager(World);
            populationManager = new PopulationManager(World);
            economyManager = new EconomyManager(World);
            relationManager = new RelationManager(World);
            geographyManager = new GeographyManager(World);

            //build countries : 
            //World.initCountries();

            World.Nations.AddRange(FileReader.ReadNationsFromCsv("../../../ESCALATION.csv"));

            foreach (Nation nation in World.Nations)
            {
                nation.initInternalStatistics(
                    (int)Random.NextDouble(), Random.NextDouble(), Random.NextDouble(), Random.NextDouble(), Random.NextDouble(), Random.NextDouble(), Random.NextDouble());
                nation.initEconomicStats( 0, 0 ,Random.Next(0, 100000));
            }

            geographyManager.initializeNeighbors("../../../neighbors.txt");
            relationManager.initAlliances();
            relationManager.initRelations();
         
          

        }

    }
}
