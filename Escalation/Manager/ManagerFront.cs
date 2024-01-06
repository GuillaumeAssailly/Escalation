using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Escalation.World;

namespace Escalation.Manager
{
    internal class ManagerFront
    {

        private IdeologyManager ideologyManager;
        private PopulationManager populationManager;
        private EconomyManager economyManager;
        private GeographyManager geographyManager;
        private RelationManager relationManager;

        private Earth World;

        public ManagerFront(Earth world)
        {
            ideologyManager = new IdeologyManager(world);
            populationManager = new PopulationManager(world);
            economyManager = new EconomyManager(world);
            geographyManager = new GeographyManager(world);
            relationManager = new RelationManager(world);
            World = world;
        }


        public void InitAll(string neighbors_path)
        {

            geographyManager.initializeNeighbors(neighbors_path);
            relationManager.initAlliances();
            relationManager.initRelations();

        }

        public void ManageIdeologies(Ecode currentNationCode)
        {
            ideologyManager.ManageIdeologies(currentNationCode);
        }

        public void ManageEconomy(Ecode currentNationCode)
        {
            economyManager.ManageEconomy(currentNationCode);
        }

        public void updateRelations(Nation currentNation)
        {
            relationManager.updateRelations(currentNation);
        }

        public void ManagePopulation(Ecode currentNationCode)
        {
            populationManager.ManagePopulation(currentNationCode);
        }

        public void EndDay()
        {
            World.CurrentEvents.Clear();
            relationManager.GoToWar();
            relationManager.ManageAlliances();
            relationManager.ManageWars();
            relationManager.ManageTension();
            World.AddDay();
        }
    }
}
