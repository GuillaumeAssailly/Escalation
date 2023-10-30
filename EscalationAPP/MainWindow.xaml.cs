using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
using Escalation.World;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Random = Escalation.Utils.Random;

namespace EscalationAPP
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public Earth World => (App.Current as App).World;

        public Random Random => (App.Current as App).Random;

        public IdeologyManager IdeologyManager => (App.Current as App).ideologyManager;


       

        public event PropertyChangedEventHandler PropertyChanged;

        private Ecode focusedNation;
        public Ecode FocusedNation
        {
            get => focusedNation;
            set => focusedNation = value;
        }

        public SeriesCollection FocusedIdeologies { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            

        }

        private void Country_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Path clickedPath = (Path)sender;
            focusedNation = (Ecode)Enum.Parse(typeof(Ecode), (string)clickedPath.Tag);
            FocusedNation = focusedNation;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FocusedNation)));

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
                    
                    Fill = Brushes.Brown
                    
                }

            };
            
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FocusedIdeologies)));

        }
    }
}
