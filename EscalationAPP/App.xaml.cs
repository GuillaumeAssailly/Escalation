using System;
using System.Linq;
using System.Windows;
using Escalation.Manager;
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


        public App()
        {
            World = new Earth();
            Random = new Random();
            ideologyManager = new IdeologyManager(World, Random);

            /////////////////////////////

       
            //Creating Managers : 
            ideologyManager = new IdeologyManager(World, Random);



            //build countries : 
            World.initCountries();
            World.setCountriesNeighbors();
         

            
            /*
            /////////////////////////////
            ///  - TESTS -  /////////////
            /////////////////////////////  
            for (int i = 0; i< 1000; i++)
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

                

            }*/
        }

    }
}
