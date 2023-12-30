using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Escalation.Utils;
using Escalation.World;
using Random = Escalation.Utils.Random;

namespace Escalation.Manager
{
    public class PopulationManager : Manager
    {
        public void ManagePopulation(Ecode code)
        {
            //Get the nation from World with the code :
            Nation CurrentNation = World.Nations[(int)code];

            double delta = ((CurrentNation.PopulationGrowthRate - CurrentNation.PopulationDeathRate) / (356 * 100));

            CurrentNation.Population += CurrentNation.Population *(decimal)delta;

            //Print the population :
            //Console.WriteLine(CurrentNation.Code + " : " + CurrentNation.Population);
            FileWriter.AppendLine(CurrentNation.Code + "/population.txt", CurrentNation.Population.ToString("0"));

            //Remove the first element of the list :
            if (CurrentNation.PopulationHistory.Count > 50)
            {
                CurrentNation.PopulationHistory.RemoveAt(0);
            }
            CurrentNation.PopulationHistory.Add(CurrentNation.Population);
           
            

        }

        public PopulationManager(Earth World, Random Random) : base(World, Random)
        {
        }
    }
}
