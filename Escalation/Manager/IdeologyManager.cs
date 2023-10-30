using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Escalation.World;
using Random = Escalation.Utils.Random;

namespace Escalation.Manager
{
    public class IdeologyManager : Manager
    {


        //Function that manage, for a given country, its ideologies, by calling the drift ideologies, and more importantly setting the rising ideology
        public void ManageIdeologies(Ecode code)
        {
            //reference of current nation :
            Nation  CurrentNation = World.Nations[(int)code];

            //How do we choose a rising ideology ?
            //First we need to know the current ideology of the nation (being the most popular one in the dictionary right now :
            // - Communism :            80% chance of being the rising ideology + what remains of chance of being a random ideology in country
            // - Socialism :            40% chance of being the rising ideology + what remains of chance of being a random ideology in country
            // - LeftWingDemocracy :    30% chance of being the rising ideology + what remains of chance of being a random neighbor ideology
            // - RightWingDemocracy :   30% chance of being the rising ideology + what remains of chance of being a random neighbor ideology
            // - Authoritarianism :     60% chance of being the rising ideology + what remains of chance of being a random ideology in country
            // - Despotism :            80% chance of being the rising ideology + what remains of chance of being a random ideology in country
            // - Fascism :              80% chance of being the rising ideology + what remains of chance of being a random neighbor in country

            //Get the  last ideology of the dictionnary (the most popular one) :
            Ideology CurrentIdeology = CurrentNation.getIdeologies().Last().Key;

            Ideology NewRisingIdeology;

            switch (CurrentIdeology)
            {
                case Ideology.Communism:
                    if (Random.NextDouble() < 0.8)
                    {
                        NewRisingIdeology = Ideology.Communism;
                    }
                    else
                    { 
                        NewRisingIdeology = CurrentNation.getIdeologies().Keys.ElementAt(Random.Next(CurrentNation.getIdeologies().Count));
                    }
                    break;
                case Ideology.Socialism:
                    if (Random.NextDouble() < 0.4)
                    {
                        NewRisingIdeology = Ideology.Socialism;
                    }
                    else
                    {
                        Nation randomNeighbor = World.Nations[(int)CurrentNation.GetNeighbors().Keys.ElementAt(Random.Next(CurrentNation.GetNeighbors().Count))];
                        NewRisingIdeology = randomNeighbor.getIdeologies().Last().Key;
                    }

                    break;
                case Ideology.LeftWingDemocracy:
                    if (Random.NextDouble() < 0.3)
                    {
                        NewRisingIdeology = Ideology.LeftWingDemocracy;
                    }
                    else
                    {
                        Nation randomNeighbor = World.Nations[(int)CurrentNation.GetNeighbors().Keys.ElementAt(Random.Next(CurrentNation.GetNeighbors().Count))];
                        NewRisingIdeology = randomNeighbor.getIdeologies().Last().Key;
                    }
                    break;
                case Ideology.RightWingDemocracy:
                    if (Random.NextDouble() < 0.3)
                    {
                        NewRisingIdeology = Ideology.LeftWingDemocracy;
                    }
                    else
                    {
                        Nation randomNeighbor = World.Nations[(int)CurrentNation.GetNeighbors().Keys.ElementAt(Random.Next(CurrentNation.GetNeighbors().Count))];
                        NewRisingIdeology = randomNeighbor.getIdeologies().Last().Key;
                    }
                    break;
                case Ideology.Authoritarianism:
                    if (Random.NextDouble() < 0.6)
                    {
                        NewRisingIdeology = Ideology.Authoritarianism;
                    }
                    else
                    {
                       NewRisingIdeology = CurrentNation.getIdeologies().Keys.ElementAt(Random.Next(CurrentNation.getIdeologies().Count));
                    }
                    break;
                case Ideology.Despotism:
                    if (Random.NextDouble() < 0.8)
                    {
                        NewRisingIdeology = Ideology.Despotism;
                    }
                    else
                    {
                        NewRisingIdeology = CurrentNation.getIdeologies().Keys.ElementAt(Random.Next(CurrentNation.getIdeologies().Count));
                    }
                    break;
                case Ideology.Fascism:
                    if (Random.NextDouble() < 0.8)
                    {
                        NewRisingIdeology = Ideology.Fascism;
                    }
                    else
                    {
                        NewRisingIdeology = CurrentNation.getIdeologies().Keys.ElementAt(Random.Next(CurrentNation.getIdeologies().Count));
                    }
                    break;
                default:
                    NewRisingIdeology = CurrentNation.getIdeologies().Keys.ElementAt(Random.Next(CurrentNation.getIdeologies().Count)); 
                    break;
            }
            

            //Percentage of the rising ideology :
            //Currently set at 0.001% of the population every day 
            //TODO : to be modified, or set it depending of the ideology !!! 
            double percentage = 0.001;    

            // Then finally set the rising ideology as needed :
            CurrentNation.SetRisingIdeology(NewRisingIdeology, percentage);



        }


        public IdeologyManager(Earth World, Random Random) : base(World, Random)
        {

        }
    }
}
