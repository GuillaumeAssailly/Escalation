using Escalation.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using Random = Escalation.Utils.Random;


namespace Escalation.World
{

    //Enum for ideologies*
    /// <summary>
    /// Communism,                      C
    /// Socialism,                      S
    /// LeftWingDemocracy,              L
    /// RightWingDemocracy,             R
    /// Authoritarianism,               A
    /// Despotism,                      D
    /// Fascism,                        F
    /// </summary>
    public enum Ideology
    {
        C,
        S,
        L,
        R,
        A,
        D,
        F
    }


    //Enum for Ecodes
    public enum Ecode
    {
        AFG, SAH, RSA, ALB, ALG, ANG, ASA, ARG, AUS, PAL, AUT, BAH, BAN,
        BEL, BEI, BEN, BHO, BIR, BOL, BOT, BRA, BRU, BUL, BUR, BUD, CAM, CAO,
        CAN, CHL, CHI, CHY, COL, NCO, SCO, COS, CIV, CUB, DAN, DDR, DJI, EGY,
        EAU, EQA, ESP, ESW,  ETU, ETH, FID, FIN, FRA, GAB, GAM, GDR, GHA,
        GRE, GUA, GUI, GEQ, GBI, GUY, FGY, HAI, HOD, HON, ISA, IND,
        IDO, IRK, IRA, IRL, ISL, ISR, ITA, JAM, JAP, JOR, KEN, KOW, LAO, LES,
        LIB, LIA, LIY, LUX, MAD, MAS, MAW, MAL, MAR, MAU, MEX, MOG, MOZ,
        NEP, NIC, NGR, NIG, NOR, NCA, NZE, OMA, OGD, PAK, PAN, PNG, PAR, PAB,
        PER, PHI, POL, PRI, POR, QAT, RCE, RDE, RDO, RDC, ROM, ROY, RUS, RVI,
        RWA, SAL, SEN, SLE, SOM,SOU, SRI, SUE, SUI, SUR, SYR,
        TAW, TAN, TCH, TCQ, THA, TOR, TOG, TET, TUN,TUR, URU, VAT,
        VEN, VIE, YEA, YEM, YOU, ZAM, ZIM,
        
    }

    //Enum for Nation Title and type (duchy, etc.)
    public enum ETitle
    {
        Republic, Kingdom, DemocraticRepublic, Duchy, Barony, County, Empire, Commune, SovietRepublic
    }


    public class Nation
    {

       
        public Ecode Code { get; set; }

        [JsonIgnore]
        public string Name { get; set; }

        [JsonIgnore]
        public string Description { get; set; }


        [JsonIgnore]
        public ETitle Title { get; set; }

        [JsonIgnore]
        //List of Neighbors with qualifiers (M for Marine, L for Land): 
        public Dictionary<Ecode, char> neighbors = new Dictionary<Ecode, char>();

        [JsonIgnore]
        private int nbWarEngagedIn;

        [JsonIgnore]
        public int NbWarEngagedIn
        {
            get => nbWarEngagedIn;
            set => nbWarEngagedIn = value < 0 ? 0 : value;
        }

        public Nation(Ecode code,
            double stability, int politicalPower, int ideology, double minIdeo, double maxIdeo, decimal population, double populationGrowthRate, double populationDeathRate,
            double populationDensity, int industrialPower, int agriculturalPower, int tertiaryPower, decimal gdp, int militaryPoints, int victoryPoints)
        {
            Code = code;
            Stability = stability;
            this.politicalPower = politicalPower;

            Population = population;
            PopulationGrowthRate = populationGrowthRate;
            PopulationDeathRate = populationDeathRate;
            PopulationDensity = populationDensity;
            PopulationHistory = new List<decimal>();

            TreasuryHistory = new List<decimal>();
            IncomesHistory = new List<decimal>();
            ExpensesHistory = new List<decimal>();
            DebtHistory = new List<decimal>();

            this.industrialPower = industrialPower;
            this.agriculturalPower = agriculturalPower;
            this.tertiaryPower = tertiaryPower;

            this._gdp = gdp;

            this.MilitaryPact = -1;
            this.Military = militaryPoints;
            this.VictoryPoints = victoryPoints;
            CurrentVictoryPoints = victoryPoints;
            
            this.SetIdeologiesRandom((Ideology)ideology,minIdeo,maxIdeo);
          
            currentPlan = (PoliticalPlan)IncomepoliticalPlans[Random.Next(IncomepoliticalPlans.Count)].Clone();

            NbWarEngagedIn = 0;
        }



        public void SetNeighbors(Dictionary<Ecode, char> neighbors)
        {
            this.neighbors = neighbors;
        }

        public Dictionary<Ecode, char> GetNeighbors()
        {
            return neighbors;
        }

        //Political Statistics
        [JsonIgnore]
        private int politicalPower;

        [JsonIgnore]
        public double Stability { get; set; }

        private Dictionary<Ideology, double> ideologies { get; set; }       //Dictionnary of ideologies to their respective percentages

        [JsonPropertyName("I")]
        public Dictionary<Ideology, double> Ideologies { get => ideologies; set => ideologies = value; }
        
        private Tuple<Ideology, double> risingIdeology;        //Tuple of the ideology that is rising and the percentage gained every day


        public void SetRisingIdeology(Ideology ideology, double percentage)
        {
            risingIdeology = new Tuple<Ideology, double>(ideology, percentage);
        }

       
        

        public void SetIdeologiesRandom(Ideology mainIdeology, double min, double max)
        {
            Ideologies = new Dictionary<Ideology, double>
                {
                    { Ideology.C, 0 },
                    { Ideology.S,0 },
                    { Ideology.L, 0 },
                    { Ideology.R, 0 },
                    { Ideology.A, 0 },
                    { Ideology.D, 0 },
                    { Ideology.F, 0 }
                };
            if (min == 0 && max == 1)
            {
                double sum = 0;
                double[] numbers = new double[7];

                for (int i = 0; i < 6; i++)
                {
                    double randomNumber = Random.NextDouble() * (1 - sum);
                    numbers[i] = randomNumber;
                    sum += randomNumber;
                }

                numbers[6] = 1 - sum;

                for (int i = numbers.Length - 1; i > 0; i--)
                {
                    int j = Random.Next(i + 1);
                    (numbers[i], numbers[j]) = (numbers[j], numbers[i]);
                }

                Ideologies[Ideology.C] = numbers[0];
                Ideologies[Ideology.S] = numbers[1];
                Ideologies[Ideology.L] = numbers[2];
                Ideologies[Ideology.R] = numbers[3];
                Ideologies[Ideology.A] = numbers[4];
                Ideologies[Ideology.D] = numbers[5];
                Ideologies[Ideology.F] = numbers[6];
            }
            else
            {
                WeightedRandomIdeologiesGenerator(mainIdeology,min,max);

            }
        }


        private void WeightedRandomIdeologiesGenerator(Ideology mainIdeology, double min, double max)
        {
            double sum = 0;
            double specificPercentage = (Random.NextDouble() * (max - min)) + min;
            sum +=specificPercentage;
            foreach (Ideology ideology in Enum.GetValues(typeof(Ideology)))
            {
                if (ideology == mainIdeology)
                {
                    Ideologies[ideology] = specificPercentage;
                }
                else
                {
                    Ideologies[ideology] = Random.NextDouble() * (1-sum);
                    sum += Ideologies[ideology];
                }
            }

            double value = Ideologies.Sum(x => x.Value);
            int a = 0;
            // shuffle the values excpet for the key containing the main ideology : 
            for (int i = Ideologies.Count-1; i > 0; i--)
            {
                if (Ideologies.ElementAt(i).Key != mainIdeology)
                {
                    int j = Random.Next(i+1);
                    while (j == (int)mainIdeology)
                    {
                        j = Random.Next(i + 1);

                    }
                    (Ideologies[(Ideology)i], Ideologies[(Ideology)j]) = (Ideologies.ElementAt(j).Value, Ideologies.ElementAt(i).Value);
                }
            }

            double value2 = Ideologies.Sum(x => x.Value);
            int a2 = 0;
        }

        public void DriftIdeologies()
        {

            //sort ideologies by ascending percentage


            //check if rising ideology progression dont go over 1 :
            if (Ideologies[risingIdeology.Item1] + risingIdeology.Item2 <= 1)
            {

                Ideologies[risingIdeology.Item1] += risingIdeology.Item2;

                int nbIdeoNotNull = Ideologies.Count(x => x.Key != risingIdeology.Item1 && x.Value > 0);

                //TODO : check if not equal to zero
                double totalToDecrease = risingIdeology.Item2 / nbIdeoNotNull;

                Ideologies = Ideologies.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);


                for (int i = 0; i < Ideologies.Count; i++)
                {
                    if (Ideologies.ElementAt(i).Key != risingIdeology.Item1 && Ideologies.ElementAt(i).Value != 0)
                    {
                        if (Ideologies.ElementAt(i).Value - totalToDecrease > 0)
                        {
                            Ideologies[Ideologies.ElementAt(i).Key] -= totalToDecrease;
                        }
                        else
                        {
                            // add the difference to the totalToDecrease divided by the number of  the remaining non zeroe ideologies
                            double delta = totalToDecrease - Ideologies.ElementAt(i).Value;
                            Ideologies[Ideologies.ElementAt(i).Key] = 0;
                            nbIdeoNotNull--;
                            //TODO : check if not equal to zero
                            totalToDecrease += delta / nbIdeoNotNull;
                        }
                    }
                    //Print current ideology value : 
                    //Console.WriteLine(ideologies.ElementAt(i).Key + " : " + ideologies.ElementAt(i).Value);

                }
            }
            else
            {
                Ideologies[risingIdeology.Item1] = 1;
                // all other are at zero
                for (int i = 0; i < Ideologies.Count; i++)
                {
                    if (Ideologies.ElementAt(i).Key != risingIdeology.Item1)
                    {
                        Ideologies[Ideologies.ElementAt(i).Key] = 0;
                    }
                }
            }

            //print the Sum of all ideologies
            //Console.WriteLine(ideologies.Sum(x => x.Value));

        }

        //Internal Statistics
        private double productivity;

        [JsonPropertyName("PR")]
        public double Productivity { get => productivity; set { if (value > 1) { productivity = 1; } else if (value < 0) { productivity = 0; } else { productivity = value; } } }
        private double educationRate;
        [JsonPropertyName("E")]
        public double EducationRate { get => educationRate; set  {if (value > 1) { educationRate = 1; } else if ( value < 0) { educationRate = 0; } else { educationRate = value; } } }
        private double healthRate;
        [JsonPropertyName("H")]
        public double HealthRate { get => healthRate; set { if ( value > 1) { healthRate = 1; } else if ( value < 0) { healthRate = 0; } else { healthRate = value; } } }
        private double happinessRate;


        [JsonIgnore]
        public double HappinessRate { get => happinessRate; set { if ( value > 1) { happinessRate = 1; } else if ( value < 0) { happinessRate = 0; } else { happinessRate = value; } } }
        private double corruptionRate;
        [JsonIgnore]
        public double CorruptionRate { get => corruptionRate; set  {if ( value > 1) { corruptionRate = 1; } else if ( value < 0) { corruptionRate = 0; } else { corruptionRate = value; } } }
        private double crimeRate;
        [JsonIgnore]
        public double CrimeRate { get => crimeRate; set { if ( value > 1) { crimeRate = 1; } else if ( value < 0) { crimeRate = 0; } else { crimeRate = value; } } }
        private double foodRate;
        [JsonIgnore]
        public double FoodRate { get => foodRate; set { if ( value > 1) { foodRate = 1; } else if ( value < 0) { foodRate = 0; } else { foodRate = value; } } }

        private int industrialPower;
        [JsonPropertyName("IP")]
        public double IndustrialPower { get => industrialPower; set => industrialPower = (int)(value < 0 ? 0 : value); }
        private int agriculturalPower;

        [JsonPropertyName("AP")]
        public double AgriculturalPower { get => agriculturalPower; set => agriculturalPower = (int)(value < 0 ? 0 : value); }
        private int tertiaryPower;

        [JsonPropertyName("TP")]
        public double TertiaryPower { get => tertiaryPower; set => tertiaryPower = (int)(value < 0 ? 0 : value); }
        private int infrastructurePower;

        private static List<PoliticalPlan> IncomepoliticalPlans = new List<PoliticalPlan>
        {
                new IndustrialPlan(),
                new AgriculturalPlan(),
                new TertiaryPlan(),
                new DismantlingAgriculturalPlan(),
                new DismantlingIndustrialPlan()
        };

        private static List<PoliticalPlan> ExpensesPoliticalPlan = new List<PoliticalPlan>
        {
            new PublicHealth(),
            new EducationPlan(),
            new FoodRatePlan(),
            new NatalityPlan(),
            new MilitaryEngagementI(),
            new MilitaryEngagementII(),
            new MilitaryEngagementIII(),

        };

        private static List<PoliticalPlan> WarPoliticalPlan = new List<PoliticalPlan>
        {
            new MilitaryEngagementI(),
            new MilitaryEngagementI(),
            new MilitaryEngagementI(),
            new MilitaryEngagementI(),
            new MilitaryEngagementI(),
            new MilitaryEngagementII(),
            new MilitaryEngagementII(),
            new MilitaryEngagementII(),
            new MilitaryEngagementII(),
            new MilitaryEngagementII(),
            new MilitaryEngagementIII(),
        };

        private static List<PoliticalPlan> AltRightPoliticalPlan = new List<PoliticalPlan>
        {
            new IndustrialPrivatisationPlan(),
            new AgriculuturalPrivatisation(),
            new DestroyingPublicService(),
        };

        private static List<PoliticalPlan> CommunistPoliticalPlan = new List<PoliticalPlan>
        {
             new Stakhanovism(),
             new FiveYearPlan(),
        };

        private static List<PoliticalPlan> SocialistPoliticalPlan = new List<PoliticalPlan>
        {
            new EcologicalPlan(),
            new PublicSocialism()
        };

        private static List<PoliticalPlan> DespoticPlan = new List<PoliticalPlan>
        {
            new DespoticRule()
        };

        private static List<PoliticalPlan> FascistPoliticalPlan = new List<PoliticalPlan>
        {
            new DestroyingSocialRights(),
            new TotalitarianRepression()

        };

        private PoliticalPlan currentPlan;

        [JsonPropertyName("CP")]
        public PoliticalPlan CurrentPlan { get => currentPlan; set => currentPlan = value; }

        public void initInternalStatistics(double productivity, double educationRate, double healthRate,
            double happinessRate, double corruptionRate, double crimeRate, double foodRate)
        {

            this.productivity = productivity;
            this.educationRate = educationRate;
            this.healthRate = healthRate;
            this.happinessRate = happinessRate;
            this.corruptionRate = corruptionRate;
            this.crimeRate = crimeRate;
            this.foodRate = foodRate;
        }

        public void takeAction()
        {
            if (CurrentPlan.isFinished())
            {
                CurrentPlan.takeEffect(this);
                CurrentPlan = null;
                //TODO: change the percentage value :
                if (NbWarEngagedIn == 0)
                {
                    if (Random.NextDouble() < 0.5) // We pick a plan depending on the current ideology : 
                    {
                        switch (Ideologies.Last().Key)
                        {
                            case Ideology.C:
                                CurrentPlan = (PoliticalPlan)CommunistPoliticalPlan[Random.Next(CommunistPoliticalPlan.Count)].Clone();
                                CurrentPlan.init();
                                break;

                            case Ideology.S:
                                CurrentPlan = (PoliticalPlan)SocialistPoliticalPlan[Random.Next(SocialistPoliticalPlan.Count)].Clone();
                                CurrentPlan.init();
                                break;

                            case Ideology.D:
                                CurrentPlan = (PoliticalPlan)DespoticPlan[Random.Next(DespoticPlan.Count)].Clone();
                                CurrentPlan.init();
                                break;

                            case Ideology.F: 
                                CurrentPlan  = (PoliticalPlan)FascistPoliticalPlan[Random.Next(FascistPoliticalPlan.Count)].Clone();
                                CurrentPlan.init();
                                break;

                            default:
                                CurrentPlan = (PoliticalPlan)AltRightPoliticalPlan[Random.Next(AltRightPoliticalPlan.Count)].Clone();
                                CurrentPlan.init();
                                break;
                        }
                    } else // We pick a plan depending on the current financial decision  :
                    {
                        if (incomes > expenses) //We pick a plan expected to decrease the economic balance :
                        {
                            currentPlan = (PoliticalPlan)ExpensesPoliticalPlan[Random.Next(ExpensesPoliticalPlan.Count)].Clone();
                            currentPlan.init();
                        } else // We pick a plan to increase the income :
                        {
                            currentPlan = (PoliticalPlan)IncomepoliticalPlans[Random.Next(IncomepoliticalPlans.Count)].Clone();
                            currentPlan.init();
                        }
                    }
                }
                else //We pick a plan for the war: 
                {
                    currentPlan = (PoliticalPlan)WarPoliticalPlan[Random.Next(WarPoliticalPlan.Count)].Clone();
                    currentPlan.init();
                }

            }
            else
            {
                currentPlan.dayPass();
            }
        }


        //Economic Statistics
        private decimal expenses;
        private decimal incomes;
        private decimal debt;
        private decimal debtInterest;
        private decimal treasury;

        private decimal _gdp;
        public decimal GDP { get => _gdp; set => _gdp = value < 0 ? 0 : value; }
        private decimal _gdpGrowthRate;

        [JsonPropertyName("GDPG")]
        public decimal GDPGrowthRate { get => _gdpGrowthRate; set => _gdpGrowthRate = value < 0 ? 0 : value; }

        [JsonPropertyName("EX")]
        public decimal Expenses { get => expenses; set => expenses = value < 0 ? 0 : value; }
        [JsonPropertyName("IN")]
        public decimal Incomes { get => incomes; set => incomes = value < 0 ? 0 : value; }
        [JsonPropertyName("DE")]
        public decimal Debt { get => debt; set => debt = value < 0 ? 0 : value; }
        [JsonIgnore]
        public decimal DebtInterest { get => debtInterest; set => debtInterest = value < 0 ? 0 : value; }
        [JsonPropertyName("TR")]
        public decimal Treasury { get => treasury; set => treasury = value < 0 ? 0 : value; }

        public void  initEconomicStats( decimal debt, decimal debtInterest, decimal treasury)
        {
             
            this.debt = debt;
            this.debtInterest = debtInterest;
            this.treasury = treasury;


        }

        public void UpdateTreasury()
        {
            //TODO : Update this 
            incomes = ((decimal)((AgriculturalPower + IndustrialPower + TertiaryPower) * (double)Population))/100000;
            expenses = ((decimal)healthRate * Population * 5 + (decimal)educationRate * Population * 1 +
                        (decimal)foodRate * Population * 1)/10000 + Military*1000;

            decimal netBalance = incomes - expenses - debtInterest;



           
           treasury += netBalance;

      
            if (treasury < 0)
            {
                debt += (decimal)Math.Abs(treasury);
                //debtInterest+= debt * 0.0000000001m;
                debtInterest = 0;
                treasury = 0; 
            }
            else
            { 
                if (treasury >= debt)
                {
                    treasury -= debt;
                    debt = 0;
                }
                else
                {
                    debt -= (decimal)netBalance;
                    // debtInterest += debt * 0.0000000001m;
                    debtInterest = 0;
                    treasury = 0;
                }
            }
            

            
            
            

            //Computing the GDP Growth Rate :
            _gdpGrowthRate = (decimal)((industrialPower + agriculturalPower + tertiaryPower) * ((productivity*4 + educationRate)/5)) /10000;

            _gdp += _gdpGrowthRate/365 * _gdp;

        }

      


        //Military Statistics
        private double manpower;                            //Percentage of population currently in the military

        private int victoryPoints;

        private ulong landForces;
        private ulong airForces;
        private ulong navalForces;
        private ulong nuclearForces;

        [JsonPropertyName("MP")]
        public int MilitaryPact { get; set; }


        private decimal military;

        [JsonPropertyName("M")]
        public decimal Military
        {
            get => military;
            set
            {
                if (value < 0)
                {
                    military = 0;
                }
                else
                {
                    military = value;
                }
            }
        }

        [JsonPropertyName("VP")]
        public int VictoryPoints { get; set; }

        private int currentVictoryPoints;

        [JsonPropertyName("CVP")]
        public int CurrentVictoryPoints
        {
            get => currentVictoryPoints;
            set => currentVictoryPoints = value < 0 ? 0 : value;
        }

        //Demographic Statistics
        private decimal _population;

        [JsonPropertyName("P")]
        public decimal Population
        {
            get => _population;
            set
            {
                if (value >= 0)
                {
                    _population = value;
                }
                else
                {
                    _population = 0;
                }


            }
        }

        [JsonPropertyName("PG")]
        public double PopulationGrowthRate { get; set; }

        [JsonPropertyName("PDR")]
        public double PopulationDeathRate { get; set; }

        [JsonIgnore]
        public double PopulationDensity { get; set; }

        [JsonIgnore]
        public List<decimal> PopulationHistory { get; set; }

        [JsonIgnore]
        public List<decimal> TreasuryHistory { get; set; }

        [JsonIgnore]
        public List<decimal> IncomesHistory { get; set; }

        [JsonIgnore]
        public List<decimal> ExpensesHistory { get; set; }

        [JsonIgnore]
        public List<decimal> DebtHistory { get; set; }
     

        //Geographic Statistics
        private ulong landArea;
        private string maincity;


        //Intelligence Statistics
        private int espionagePoints;
        private int counterEspionagePoints;


   
    }
}
