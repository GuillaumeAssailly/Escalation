using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Escalation.Manager;
using Escalation.Utils;
using Escalation.World;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using Random = Escalation.Utils.Random;

namespace EscalationAPP
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        //TODO : Fix the crash when exiting the app as Application could be Null and then World as Well

        /// <summary>
        /// ATTRIBUTES OBJECTS : 
        /// </summary>
        public Earth World => (Application.Current as App)?.World;

        public DateTime CurrentDate => World.CurrentDate;
        public Random Random => (App.Current as App).Random;
        public IdeologyManager IdeologyManager => (App.Current as App).ideologyManager;
        public PopulationManager PopulationManager => (App.Current as App).populationManager;

        private double _axisMax;
        private double _axisMin;
        private double _axisMaxY;
        public double AxisMin
        {
            get { return _axisMin; }
            set
            {
                _axisMin = value;
                OnPropertyChanged("AxisMin");
            }
        }
        public double AxisMax
        {
            get { return _axisMax; }
            set
            {
                _axisMax = value;
                OnPropertyChanged("AxisMax");
            }
        }

        public double AxisMaxY
        {
            get { return _axisMaxY; }
            set
            {
                _axisMaxY = value;
                OnPropertyChanged("AxisMaxY");
            }
        }
        public double AxisStep { get; set; }

        /// <summary>
        /// EVENTS : 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private CancellationTokenSource pauseTokenSource = new CancellationTokenSource();
        private CancellationToken pauseToken;
        private bool isPaused = false;
        private int speed = 2;

        /// <summary>
        /// CURRENT NATION FOCUSED :
        /// </summary>
        private Ecode focusedNation;
        public Ecode FocusedNation
        {
            get => focusedNation;
            set => focusedNation = value;
        }

        public SeriesCollection FocusedIdeologies { get; set; }
        public SeriesCollection FocusedPopulation { get; set; }

      

        public ChartValues<decimal> HistoPopulation { get; set; }
       

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            pauseToken = pauseTokenSource.Token;

            //Events:
            PreviewKeyDown += MainWindow_PreviewKeyDown;
            AxisStep = 10;
            AxisMax = 50;
            AxisMin = 0;
            AxisMaxY = 100;

            //Initializing the focused nation :
            FocusedNation = Ecode.FRA;
            HistoPopulation = new ChartValues<decimal>();

            //Launch the tests in an other thread :
            launch();

            //Construction of UI elements : 
            initChart();
            initPopGraph();

           



        }

        ///////////////////////////////////////THREADS : ///////////////////////////////////////
        private async void launch()
        {
            await Task.Run(() => 
            {
                /////////////////////////////
                /// - FILES CREATION -  /////
                /////////////////////////////
                foreach (Nation n in World.Nations)
                {
                    FileWriter.CreateFiles("", n.Code);
                }

                /////////////////////////////
                ///  - TESTS -  /////////////
                /////////////////////////////  
                for (int i = 0; i < 100000; i++)
                {

                    if (isPaused)
                    {
                        while (isPaused)
                        {
                            pauseToken.ThrowIfCancellationRequested(); // This will throw if the task has been cancelled.
                            Thread.Sleep(100);
                        }
                    }

                    if (i % 10 == 0)
                    {
                        //browse each nations in the world with a foreach loop : 
                        foreach (Nation currentNation in World.Nations)
                        {
                            IdeologyManager.ManageIdeologies(currentNation.Code);
                        }

                    }


                    foreach (Nation currentNation in World.Nations)
                    {
                        PopulationManager.ManagePopulation(currentNation.Code);
                        currentNation.DriftIdeologies();
                        //Print majorIdeology in each nation : 
                        Console.WriteLine(currentNation.Code + " : " + currentNation.getIdeologies().Last().Key + " with " + currentNation.getIdeologies().Last().Value);
                    }

                    World.AddDay();


                    //Delay of 1 second :
                    Thread.Sleep(speed * 500+5);

                    Application.Current?.Dispatcher.Invoke(new Action(UpdateUI));

                    pauseToken.ThrowIfCancellationRequested(); // This will throw if the task has been cancelled.


                }
            });

        }

      

        ///////////////////////////////////////UI : ///////////////////////////////////////

        private void UpdateUI()
        {
            DateBlock.Text = CurrentDate.ToString("dd/MM/yyyy");
            UpdateChart();
            UpdatePopGraph();

        }


        private void UpdateChart()
        {
            Dictionary<Ideology, double> ideologies = World.Nations[(int)focusedNation].getIdeologies();

            FocusedIdeologies.ElementAt(0).Values.Cast<ObservableValue>().ElementAt(0).Value = ideologies[Ideology.Communism] * 100;
            FocusedIdeologies.ElementAt(1).Values.Cast<ObservableValue>().ElementAt(0).Value = ideologies[Ideology.Socialism] * 100;
            FocusedIdeologies.ElementAt(2).Values.Cast<ObservableValue>().ElementAt(0).Value = ideologies[Ideology.LeftWingDemocracy] * 100;
            FocusedIdeologies.ElementAt(3).Values.Cast<ObservableValue>().ElementAt(0).Value = ideologies[Ideology.RightWingDemocracy] * 100;
            FocusedIdeologies.ElementAt(4).Values.Cast<ObservableValue>().ElementAt(0).Value = ideologies[Ideology.Authoritarianism] * 100;
            FocusedIdeologies.ElementAt(5).Values.Cast<ObservableValue>().ElementAt(0).Value = ideologies[Ideology.Despotism] * 100;
            FocusedIdeologies.ElementAt(6).Values.Cast<ObservableValue>().ElementAt(0).Value = ideologies[Ideology.Fascism] * 100;

        }
       
        private void UpdatePopGraph()
        {
            AxisMaxY = (double)World.Nations[(int)FocusedNation].Population*1.8;
            if (HistoPopulation.Count > 50)
            {
                HistoPopulation.RemoveAt(0);
            }
            HistoPopulation.Add(World.Nations[(int)FocusedNation].Population);
         
        }

        private void initChart()
        {
            Dictionary<Ideology, double> ideologies = World.Nations[(int)focusedNation].getIdeologies();

            
            FocusedIdeologies = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Communism",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(ideologies[Ideology.Communism] * 100) },
                    Fill = Brushes.DarkRed
                },
                new PieSeries
                {
                    Title = "Socialism",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(ideologies[Ideology.Socialism] * 100) },
                    Fill = Brushes.Red
                },
                new PieSeries
                {
                    Title = "LeftWingDemocracy",
                    Values = new ChartValues<ObservableValue>
                        { new ObservableValue(ideologies[Ideology.LeftWingDemocracy] * 100) },
                    Fill = Brushes.Pink
                },
                new PieSeries
                {
                    Title = "RightWingDemocracy",
                    Values = new ChartValues<ObservableValue>
                        { new ObservableValue(ideologies[Ideology.RightWingDemocracy]*100) },
                    Fill = Brushes.LightBlue
                },
                new PieSeries
                {
                    Title = "Authoritarianism",
                    Values = new ChartValues<ObservableValue>
                        { new ObservableValue(ideologies[Ideology.Authoritarianism] * 100) },
                    Fill = Brushes.Gray
                },
                new PieSeries
                {
                    Title = "Despotism",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(ideologies[Ideology.Despotism] * 100) },
                    Fill = Brushes.Black
                },
                new PieSeries
                {
                    Title = "Fascism",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(ideologies[Ideology.Fascism]*100) },

                    Fill = Brushes.SaddleBrown

                }

            };
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FocusedIdeologies)));
        }

        void initPopGraph()
        {
            //HistoPopulation = FileReader.ReadPopulation(FocusedNation.ToString() + "/population.txt").AsChartValues();

            HistoPopulation = World.Nations[(int)FocusedNation].PopulationHistory.AsChartValues();

            FocusedPopulation = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Population",
                    Values = HistoPopulation,
                    PointGeometry = null,
                    Fill = Brushes.Transparent
                }
            };
           
        }


        private void initPopGraphOnCountryChange()
        {

            HistoPopulation = World.Nations[(int)FocusedNation].PopulationHistory.AsChartValues();
            FocusedPopulation.ElementAt(0).Values = HistoPopulation;
        }

        ///////////////////////////////////////EVENTS : ///////////////////////////////////////
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null) 
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Country_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Path clickedPath = (Path)sender;
            focusedNation = (Ecode)Enum.Parse(typeof(Ecode), (string)clickedPath.Tag);
            FocusedNation = focusedNation;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FocusedNation)));




            initPopGraphOnCountryChange();
            UpdateUI();

        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    isPaused = !isPaused; // Toggle the pause flag

                    if (!isPaused)
                    {
                        // Unpause the task (if it was paused)
                        pauseTokenSource = new CancellationTokenSource();
                        pauseToken = pauseTokenSource.Token;
                    }
                    break;
                case Key.Escape:
                    Close();
                    break;
                case Key.Subtract:
                    if (speed < 5) { speed++;}
                    break;
                case Key.Add:                   
                    if (speed > 0) { speed--; }
                    break;
                case Key.X:
                    World.Nations[(int)FocusedNation].Population -= Random.Next(0, 10000000);
                    break;
                case Key.C:
                    World.Nations[(int)FocusedNation].Population += Random.Next(0, 10000000);
                    break;
            }
            {
                
            }
        }
    }
}
