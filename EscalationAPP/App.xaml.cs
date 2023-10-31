using System;
using System.Linq;
using System.Threading;
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
         

           
        }

    }
}
