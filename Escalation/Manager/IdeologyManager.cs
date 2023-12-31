﻿using System;
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
            // - Communism :            X% chance of being the rising ideology + what remains of chance of being a random ideology in country
            // - Socialism :            X% chance of being the rising ideology + what remains of chance of being a random ideology in country
            // - LeftWingDemocracy :    X% chance of being the rising ideology + what remains of chance of being a random neighbor ideology
            // - RightWingDemocracy :   X% chance of being the rising ideology + what remains of chance of being a random neighbor ideology
            // - Authoritarianism :     X% chance of being the rising ideology + what remains of chance of being a random ideology in country
            // - Despotism :            X% chance of being the rising ideology + what remains of chance of being a random ideology in country
            // - Fascism :              X% chance of being the rising ideology + what remains of chance of being a random neighbor in country

            //Get the  last ideology of the dictionary (the most popular one) :
            Ideology CurrentIdeology = CurrentNation.Ideologies.Last().Key;

            Ideology NewRisingIdeology;

            switch (CurrentIdeology)
            {
                case Ideology.C:
                    if (Random.NextDouble() < CurrentNation.Stability)
                    {
                        NewRisingIdeology = Ideology.C;
                    }
                    else
                    { 
                        NewRisingIdeology = CurrentNation.Ideologies.Keys.ElementAt(Random.Next(CurrentNation.Ideologies.Count));
                    }
                    break;
                case Ideology.S:
                    if (Random.NextDouble() < CurrentNation.Stability)
                    {
                        NewRisingIdeology = Ideology.S;
                    }
                    else
                    {
                        NewRisingIdeology = CurrentNation.Ideologies.Keys.ElementAt(Random.Next(CurrentNation.Ideologies.Count));
                    }
                    break;
                case Ideology.L:
                    if (Random.NextDouble() < CurrentNation.Stability)
                    {
                        NewRisingIdeology = Ideology.L;
                    }
                    else
                    {
                        NewRisingIdeology = CurrentNation.Ideologies.Keys.ElementAt(Random.Next(CurrentNation.Ideologies.Count));
                    }
                    break;
                case Ideology.R:
                    if (Random.NextDouble() < CurrentNation.Stability)
                    {
                        NewRisingIdeology = Ideology.R;
                    }
                    else
                    {
                        NewRisingIdeology = CurrentNation.Ideologies.Keys.ElementAt(Random.Next(CurrentNation.Ideologies.Count));
                    }
                    break;
                case Ideology.A:
                    if (Random.NextDouble() < CurrentNation.Stability)
                    {
                        NewRisingIdeology = Ideology.A;
                    }
                    else
                    {
                       NewRisingIdeology = CurrentNation.Ideologies.Keys.ElementAt(Random.Next(CurrentNation.Ideologies.Count));
                    }
                    break;
                case Ideology.D:
                    if (Random.NextDouble() < CurrentNation.Stability)
                    {
                        NewRisingIdeology = Ideology.D;
                    }
                    else
                    {
                        NewRisingIdeology = CurrentNation.Ideologies.Keys.ElementAt(Random.Next(CurrentNation.Ideologies.Count));
                    }
                    break;
                case Ideology.F:
                    if (Random.NextDouble() < CurrentNation.Stability)
                    {
                        NewRisingIdeology = Ideology.F;
                    }
                    else
                    {
                        NewRisingIdeology = CurrentNation.Ideologies.Keys.ElementAt(Random.Next(CurrentNation.Ideologies.Count));
                    }
                    break;
                default:
                    NewRisingIdeology = CurrentNation.Ideologies.Keys.ElementAt(Random.Next(CurrentNation.Ideologies.Count)); 
                    break;
            }
            

            //Percentage of the rising ideology :
            //Currently set at 0.001% of the population every day 
            //TODO : to be modified, or set it depending of the ideology !!! 
            double percentage = 0.001;    

            // Then finally set the rising ideology as needed :
            CurrentNation.SetRisingIdeology(NewRisingIdeology, percentage);



        }


        public IdeologyManager(Earth World) : base(World)
        {

        }
    }
}
