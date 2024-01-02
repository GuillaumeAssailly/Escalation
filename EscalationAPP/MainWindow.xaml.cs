using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Escalation.Manager;
using Escalation.Utils;
using Escalation.World;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using Wpf.Ui.Controls;
using Path = System.Windows.Shapes.Path;
using Random = Escalation.Utils.Random;


namespace EscalationAPP
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    ///
    ///

    public class MyTraceListener : TraceListener
    {
        private readonly System.Windows.Controls.TextBox _textBox;

        public MyTraceListener(System.Windows.Controls.TextBox textBlock)
        {
            _textBox = textBlock;
        }


        public override void Write(string message)
        {
            throw new NotImplementedException();
        }

        public override void WriteLine(string message)
        {
            //_textBox.Dispatcher.Invoke(() => _textBox.Text += message + "\n");
            
        }
    }



    public partial class MainWindow : FluentWindow, INotifyPropertyChanged
    {

        //TODO : Fix the crash when exiting the app as Application could be Null and then World as Well

        /// <summary>
        /// ATTRIBUTES OBJECTS : 
        /// </summary>
        ///
        ///
        ///

        public ObservableCollection<War> ListOfWars { get; set; }


        public Earth World => (Application.Current as App)?.World;

        public DateTime CurrentDate => World.CurrentDate;
        public Random Random => (App.Current as App).Random;
        public IdeologyManager IdeologyManager => (App.Current as App).ideologyManager;
        public PopulationManager PopulationManager => (App.Current as App).populationManager;
        public EconomyManager EconomyManager => (App.Current as App).economyManager;

        public GeographyManager GeographyManager => (App.Current as App).geographyManager;

        public RelationManager RelationManager => (App.Current as App).relationManager;


        private Ecode? countryMouseEntered = null;
        private Dictionary<Ecode, SolidColorBrush> CountryLayer;


        private int layerMode = 0;
    
        private double _axisMaxY;
        public double AxisMaxY
        {
            get => _axisMaxY;
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
        public int Speed
        {
            get => speed;
            set
            {
                speed = value;
                OnPropertyChanged("Speed");
            }
        }

        /// <summary>
        /// CURRENT NATION FOCUSED :
        /// </summary>
        private Ecode focusedNation;
        public Ecode FocusedNation
        {
            get => focusedNation;
            set => focusedNation = value;
        }

        private Ecode focusedNationRelation;

        public Ecode FocusedNationRelation
        {
            get => focusedNationRelation;
            set => focusedNationRelation = value;
        } 

        public SeriesCollection FocusedIdeologies { get; set; }
        public SeriesCollection FocusedPopulation { get; set; }

        public SeriesCollection FocusedEconomy { get; set; }
      

        public ChartValues<decimal> HistoPopulation { get; set; }
        public ChartValues<decimal> HistoTreasury { get; set; }
        public ChartValues<decimal> HistoExpenses {get; set; }
        public ChartValues<decimal> HistoIncomes { get; set; }
        public ChartValues<decimal> HistoDebts { get; set; }

    


        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            var traceListener = new MyTraceListener(outputTextBox);
            Trace.Listeners.Add(traceListener);
            Trace.AutoFlush = true;

            pauseToken = pauseTokenSource.Token;

            //Events:
            PreviewKeyDown += MainWindow_PreviewKeyDown;
            AxisStep = 10;
          
            AxisMaxY = 100;

            //Initializing the focused nation :
            FocusedNation = Ecode.FRA;
            HistoPopulation = new ChartValues<decimal>();

            //Initializing the country Layer :
            CountryLayer = new Dictionary<Ecode, SolidColorBrush>();         

            //Launch the tests in an other thread :
            launch();

            //Construction of UI elements : 
            initChart();
            initPopGraph();
            initEconomyGraph();

            
            foreach (Alliance a in World.Alliances)
            {
                foreach (Nation n in a.GetMembers())
                {
                    CountryLayer.Add(n.Code, new SolidColorBrush((Color)ColorConverter.ConvertFromString(a.color)));

                }
            }

            
          

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
                    FileWriter.SaveIdeologies("idee.txt", n.getIdeologies());
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


                     
                  
                     
                    //DAY LOOP OVER HERE :
                    foreach (Nation currentNation in World.Nations)
                    {
                        if (i % 10 == 0)
                        {
                            IdeologyManager.ManageIdeologies(currentNation.Code);
                        }


                        if (World.CurrentDate.Day == 1)
                        {
                            EconomyManager.ManageEconomy(currentNation.Code);
                            RelationManager.updateRelations(currentNation); 
                            currentNation.DriftIdeologies();
                        }

                        //print the 20 richest countries : order by treasury :
                       // List<Nation> richestCountries = World.Nations.OrderByDescending(o => o.Treasury).ToList();
                       

                       

                        PopulationManager.ManagePopulation(currentNation.Code);
                        
                        
                        //Print majorIdeology in each nation : 
                        //Console.WriteLine(currentNation.Code + " : " + currentNation.getIdeologies().Last().Key + " with " + currentNation.getIdeologies().Last().Value);
                        currentNation.takeAction();
                    }

                    RelationManager.ManageAlliances();
                    RelationManager.GoToWar();
                    RelationManager.ManageWars();
                    RelationManager.ManageTension();
                    World.AddDay();

                    //Delay of 1 second :
                    Thread.Sleep(speed * 200+5);

                    Application.Current?.Dispatcher.Invoke(new Action(UpdateUI));

                    pauseToken.ThrowIfCancellationRequested(); // This will throw if the task has been cancelled.

                    //Trace.WriteLine(JsonSerializer.Serialize(World)));

                }
            });

        }

      

        ///////////////////////////////////////UI : ///////////////////////////////////////

        private void UpdateLayer()
        {
            foreach (KeyValuePair<Ecode, SolidColorBrush> entry in CountryLayer)
            {

                //get the path with the tag :
                if ((Ecode)entry.Key != countryMouseEntered)
                {
                    Path country = (Path)FindName(entry.Key.ToString());
                    //set the color :
                    country.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#666666"));
                }

            }
            CountryLayer.Clear();

            if (layerMode == 0)
            {
                foreach (Alliance a in World.Alliances)
                {
                    foreach (Nation n in a.GetMembers())
                    {
                        CountryLayer.Add(n.Code, new SolidColorBrush((Color)ColorConverter.ConvertFromString(a.color)));
                    }
                }
            }
            else if (layerMode == 1)
            {
                foreach (War w in World.Wars)
                {
                    if (w.Attackers.Contains(World.Nations[(int)FocusedNation]))
                    {
                        foreach (Nation n in w.Defenders)
                        {
                            if (CountryLayer.ContainsKey(n.Code))
                            {
                                continue;
                            }
                            CountryLayer.Add(n.Code, Brushes.Red);
                        }
                        foreach (Nation n in w.Attackers)
                        {
                            if (CountryLayer.ContainsKey(n.Code))
                            {
                                continue;
                            }
                            CountryLayer.Add(n.Code, Brushes.Green);
                        }
                    }
                    if (w.Defenders.Contains(World.Nations[(int)FocusedNation]))
                    {
                        foreach (Nation n in w.Attackers)
                        {
                            if (CountryLayer.ContainsKey(n.Code))
                            {
                                continue;
                            }
                            CountryLayer.Add(n.Code, Brushes.Red);
                        }
                        foreach (Nation n in w.Defenders)
                        {
                            if (CountryLayer.ContainsKey(n.Code))
                            {
                                continue;
                            }
                            CountryLayer.Add(n.Code, Brushes.Green);
                        }
                    }


                }
            }
            else if (layerMode == 2)
            {
                foreach(Nation n in World.Nations)
                {
                    if (n.Code == FocusedNation)
                    {
                        CountryLayer.Add(n.Code, Brushes.Blue); ;
                    }
                    else if (!World.WarMatrix[(int)n.Code, (int)FocusedNation] )
                    {
                        CountryLayer.Add(n.Code, Brushes.Green);
                    }
                    else
                    {
                        CountryLayer.Add(n.Code, Brushes.Red);
                    }
                }
            }
            foreach (KeyValuePair<Ecode, SolidColorBrush> entry in CountryLayer)
            {
                //get the path with the tag :
                if ((Ecode)entry.Key != countryMouseEntered)
                {
                    Path country = (Path)FindName(entry.Key.ToString());
                    //set the color :
                    country.Fill = entry.Value;
                }
            }

        }

        private void UpdateUI()
        {
            DateBlock.Text = CurrentDate.ToString("dd/MM/yyyy");
            WorldTension.Text = World.WorldTension.ToString("00.000");
            UpdateInternalDetails();
            UpdateChart();
            UpdatePopGraph();
            UpdateEconomyGraph();
            //UpdateNeighboor();
            UpdateLayer();
            outputTextBox.ScrollToEnd();
            ListOfWars = new ObservableCollection<War>(World.Wars);
            
        }

        private void initNeighboorOnCountryChange()
        {
            foreach (KeyValuePair<Ecode, SolidColorBrush> entry in CountryLayer)
            {
                //get the path with the tag :
                if ((Ecode)entry.Key != countryMouseEntered)
                {
                    Path country = (Path)FindName(entry.Key.ToString());
                    //set the color :
                    country.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#666666"));
                }

            }
        }

        private void UpdateNeighboor()
        {
          
            CountryLayer.Clear();
            foreach (Ecode e in World.Nations[(int)FocusedNation].neighbors.Keys)
            {
               CountryLayer.Add(e, Brushes.Green);
            }
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

        private void UpdateEconomyGraph()
        {
            if (World.CurrentDate.Day == 1)
            {
                if (HistoTreasury.Count > 12)
                {
                    HistoTreasury.RemoveAt(0);
                    HistoExpenses.RemoveAt(0);
                    HistoIncomes.RemoveAt(0);
                    HistoDebts.RemoveAt(0);
                }
                HistoTreasury.Add(World.Nations[(int)FocusedNation].Treasury);
                HistoExpenses.Add(World.Nations[(int)FocusedNation].Expenses);
                HistoIncomes.Add(World.Nations[(int)FocusedNation].Incomes);
                HistoDebts.Add(World.Nations[(int)FocusedNation].Debt);
            }
           
        }

        private void UpdateInternalDetails()
        {
            CountryAgriculturalPower.Text = World.Nations[(int)FocusedNation].AgriculturalPower.ToString();
            CountryIndustrialPower.Text = World.Nations[(int)FocusedNation].IndustrialPower.ToString();
            CountryTertiaryPower.Text = World.Nations[(int)FocusedNation].TertiaryPower.ToString();
            CountryMilitary.Text = World.Nations[(int)FocusedNation].Military.ToString();
            CountryVP.Text = World.Nations[(int)FocusedNation].CurrentVictoryPoints.ToString();
            CountryGDPGROWTH.Text = World.Nations[(int)FocusedNation].GDPGrowthRate.ToString();
            CountryGDP.Text = World.Nations[(int)FocusedNation].GDP.ToString();
            CountryEducationRate.Text = "Education Rate :" + World.Nations[(int)FocusedNation].EducationRate.ToString();
            CountryHealthCare.Text = "Health Care  : " + World.Nations[(int)FocusedNation].HealthRate.ToString();
            CountryProductivity.Text = "Productivity : " + World.Nations[(int)FocusedNation].Productivity.ToString();
            CountryCrimeRate.Text = "Crime Rate :" + World.Nations[(int)FocusedNation].CrimeRate.ToString();
            CountryFoodRate.Text = "Food Rate :" + World.Nations[(int)FocusedNation].FoodRate.ToString();
            CountryHappiness.Text = "Happiness : " + World.Nations[(int)FocusedNation].HappinessRate.ToString();
            CountryCorruptionRate.Text = "Corruption :" + World.Nations[(int)FocusedNation].CorruptionRate.ToString();

            CurrentPlan.Text = World.Nations[(int)FocusedNation].CurrentPlan.GetDescription().ToString();
            CurrentPlanProgress.Value = World.Nations[(int)FocusedNation].CurrentPlan.GetProgress() * 100;
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
                    Fill = Brushes.DarkRed,
                    Stroke = Brushes.Transparent 
                },
                new PieSeries
                {
                    Title = "Socialism",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(ideologies[Ideology.Socialism] * 100) },
                    Fill = Brushes.Red,
                    Stroke = Brushes.Transparent

                },
                new PieSeries
                {
                    Title = "LeftWingDemocracy",
                    Values = new ChartValues<ObservableValue>
                        { new ObservableValue(ideologies[Ideology.LeftWingDemocracy] * 100) },
                    Fill = Brushes.Pink,
                    Stroke = Brushes.Transparent

                },
                new PieSeries
                {
                    Title = "RightWingDemocracy",
                    Values = new ChartValues<ObservableValue>
                        { new ObservableValue(ideologies[Ideology.RightWingDemocracy]*100) },
                    Fill = Brushes.LightBlue,
                    Stroke = Brushes.Transparent

                },
                new PieSeries
                {
                    Title = "Authoritarianism",
                    Values = new ChartValues<ObservableValue>
                        { new ObservableValue(ideologies[Ideology.Authoritarianism] * 100) },
                    Fill = Brushes.Gray,
                    Stroke = Brushes.Transparent

                },
                new PieSeries
                {
                    Title = "Despotism",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(ideologies[Ideology.Despotism] * 100) },
                    Fill = Brushes.Black,
                    Stroke = Brushes.Transparent 
                },
                new PieSeries
                {
                    Title = "Fascism",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(ideologies[Ideology.Fascism]*100) },

                    Fill = Brushes.SaddleBrown,
                    Stroke = Brushes.Transparent

                }

            };
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FocusedIdeologies)));
        }

        void initPopGraph()
        {
            //HistoPopulation = FileReader.ReadPopulation(FocusedNation.ToString() + "/population.txt").AsChartValues();

            HistoTreasury = World.Nations[(int)FocusedNation].PopulationHistory.AsChartValues();

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
            HistoPopulation  = World.Nations[(int)FocusedNation].PopulationHistory.AsChartValues();
            FocusedPopulation.ElementAt(0).Values = HistoPopulation;
        }

        private void initEconomyGraphOnCountryChange()
        {
            HistoTreasury = World.Nations[(int)FocusedNation].TreasuryHistory.AsChartValues();
            HistoExpenses = World.Nations[(int)FocusedNation].ExpensesHistory.AsChartValues();
            HistoIncomes = World.Nations[(int)FocusedNation].IncomesHistory.AsChartValues();
            HistoDebts = World.Nations[(int)FocusedNation].DebtHistory.AsChartValues();

            FocusedEconomy.ElementAt(0).Values = HistoTreasury;
            FocusedEconomy.ElementAt(1).Values = HistoExpenses;
            FocusedEconomy.ElementAt(2).Values = HistoIncomes;
            FocusedEconomy.ElementAt(3).Values = HistoDebts;
        }

        public void initEconomyGraph()
        {
            HistoTreasury = World.Nations[(int)FocusedNation].TreasuryHistory.AsChartValues();
            HistoExpenses = World.Nations[(int)FocusedNation].ExpensesHistory.AsChartValues();
            HistoIncomes = World.Nations[(int)FocusedNation].IncomesHistory.AsChartValues();
            HistoDebts = World.Nations[(int)FocusedNation].DebtHistory.AsChartValues();

            FocusedEconomy = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Treasury",
                    Values = HistoTreasury,
                    Stroke = Brushes.Red,
                    Fill = Brushes.Transparent,
                    StrokeThickness = 4
                },
                new LineSeries
                {
                    Title = "Expenses",
                    Values = HistoExpenses,
                    Stroke = Brushes.Blue,
                    Fill = Brushes.Transparent,
                    StrokeThickness = 4
                },
                new LineSeries
                {
                    Title = "Incomes",
                    Values = HistoIncomes,
                    Stroke = Brushes.Green,
                    Fill = Brushes.Transparent,
                    StrokeThickness = 4
                },
                new LineSeries
                {
                    Title = "Debts",
                    Values = HistoDebts,
                    Stroke = Brushes.Black,
                    Fill = Brushes.Transparent,
                    StrokeThickness = 4
                }

            };
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
            initEconomyGraphOnCountryChange();
            initNeighboorOnCountryChange();
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
                case Key.F1:
                    layerMode = 0; UpdateLayer();
                    break;
                case Key.F2:
                    layerMode = 1; UpdateLayer();
                    break;
                case Key:
                    layerMode = 2; UpdateLayer();
                    break;
            }
            {
                
            }
        }


        private void OnCountryMouseEnter(object sender, MouseEventArgs e)
        {
            Path p = (Path)sender;
            //parse string to Ecode :
            countryMouseEntered = (Ecode)Enum.Parse(typeof(Ecode), (string)p.Tag);
        }

        private void OnCountryMouseLeave(object sender, MouseEventArgs e)
        {
            SolidColorBrush targetBrush = null;
            countryMouseEntered = null;
            Path p = (Path)sender;

            // If p.name exists in the dictionary
            if (CountryLayer.ContainsKey((Ecode)Enum.Parse(typeof(Ecode), (string)p.Tag)))
            {
                // Use the color from the dictionary
                targetBrush = CountryLayer[(Ecode)Enum.Parse(typeof(Ecode), (string)p.Tag)];
                    

            }
            else
            {
                // Use brush #666666 with a smooth transition
                targetBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#666666"));
              
            }

            ColorAnimation colorAnimation = new ColorAnimation(targetBrush.Color, TimeSpan.FromMilliseconds(200));

            p.Fill.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
        }

        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void OnMouserRightButtonUp(object sender, MouseButtonEventArgs e)
        {
           
            
        }
    }

    
}
