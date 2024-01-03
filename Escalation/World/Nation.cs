using Escalation.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using Random = Escalation.Utils.Random;


namespace Escalation.World
{

    //Enum for ideologies
    public enum Ideology
    {
        Communism,
        Socialism,
        LeftWingDemocracy,
        RightWingDemocracy,
        Authoritarianism,
        Despotism,
        Fascism,
    }


    //Enum for Ecodes
    public enum Ecode
    {
        AFG, SAH, RSA, ALB, ALG, AND, ANG, ASA, ARG, AUS, PAL, AUT, BAH, BAN,
        BEL, BEI, BEN, BHO, BIR, BOL, BOT, BRA, BRU, BUL, BUR, BUD, CAM, CAO,
        CAN, CHL, CHI, CHY, COL, NCO, SCO, COS, CIV, CUB, DAN, DDR, DJI, EGY,
        EAU, EQA, ESP, ESW,  ETU, ETH, FID, FIN, FRA, GAB, GAM, GDR, GHA,
        GRE, GRO, GUA, GUI, GEQ, GBI, GUY, FGY, HAI, HOD, HON, IMA, ISA, IND,
        IDO, IRK, IRA, IRL, ISL, ISR, ITA, JAM, JAP, JOR, KEN, KOW, LAO, LES,
        LIB, LIA, LIY, LUX, MAD, MAS, MAW, MAL, MAR, MAU, MEX, MON, MOG, MOZ,
        NEP, NIC, NGR, NIG, NOR, NCA, NZE, OMA, OGD, PAK, PAN, PNG, PAR, PAB,
        PER, PHI, POL, PRI, POR, QAT, RCE, RDE, RDO, RDC, ROM, ROY, RUS, RVI,
        RWA, SAL, SEN, SLE, SOM,SOU, SDS, SRI, SUE, SUI, SUR, SVA, SYR,
        TAW, TAN, TCH, TCQ, TAA, THA, TOR, TOG, TET, TUN,TUR, URU, VAN, VAT,
        VEN, VIE, YEA, YEM, YOU, ZAM, ZIM,

        /*
                FRA = 0, 
                ALL,
                ITA,
                ROY,
                ESP,
                IRL,
                POR,
                BEL,
                PAB,
                SUI,
                LUX,*/


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
        public string Flag { get; set; }

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
        private Tuple<Ideology, double> risingIdeology;        //Tuple of the ideology that is rising and the percentage gained every day


        public void SetRisingIdeology(Ideology ideology, double percentage)
        {
            risingIdeology = new Tuple<Ideology, double>(ideology, percentage);
        }

        public Dictionary<Ideology, double> getIdeologies()
        {
            return ideologies;
        }

        public void SetIdeologies(Dictionary<Ideology, double> ideologies)
        {
            this.ideologies = ideologies;
        }

        public void SetIdeologiesRandom(Ideology mainIdeology, double min, double max)
        {
            ideologies = new Dictionary<Ideology, double>
                {
                    { Ideology.Communism, 0 },
                    { Ideology.Socialism,0 },
                    { Ideology.LeftWingDemocracy, 0 },
                    { Ideology.RightWingDemocracy, 0 },
                    { Ideology.Authoritarianism, 0 },
                    { Ideology.Despotism, 0 },
                    { Ideology.Fascism, 0 }
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

                ideologies[Ideology.Communism] = numbers[0];
                ideologies[Ideology.Socialism] = numbers[1];
                ideologies[Ideology.LeftWingDemocracy] = numbers[2];
                ideologies[Ideology.RightWingDemocracy] = numbers[3];
                ideologies[Ideology.Authoritarianism] = numbers[4];
                ideologies[Ideology.Despotism] = numbers[5];
                ideologies[Ideology.Fascism] = numbers[6];
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

            foreach (Ideology ideology in Enum.GetValues(typeof(Ideology)))
            {
                if (ideology == mainIdeology)
                {
                    ideologies[ideology] = specificPercentage;
                    sum+= specificPercentage;
                }
                else
                {
                    ideologies[ideology] = Random.NextDouble() * (1 - sum);
                    sum += ideologies[ideology];
                }
            }

            // shuffle the values excpet for the key containing the main ideology : 
            for (int i = ideologies.Count-1; i > 0; i--)
            {
                if (ideologies.ElementAt(i).Key != mainIdeology)
                {
                    int j = Random.Next(i+1);
                    while (j == (int)mainIdeology)
                    {
                        j = Random.Next(i + 1);

                    }
                    (ideologies[(Ideology)i], ideologies[(Ideology)j]) = (ideologies.ElementAt(j).Value, ideologies.ElementAt(i).Value);
                }
            }


        }

        public void DriftIdeologies()
        {

            //sort ideologies by ascending percentage


            //check if rising ideology progression dont go over 1 :
            if (ideologies[risingIdeology.Item1] + risingIdeology.Item2 <= 1)
            {

                ideologies[risingIdeology.Item1] += risingIdeology.Item2;

                int nbIdeoNotNull = ideologies.Count(x => x.Key != risingIdeology.Item1 && x.Value > 0);

                //TODO : check if not equal to zero
                double totalToDecrease = risingIdeology.Item2 / nbIdeoNotNull;

                ideologies = ideologies.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);


                for (int i = 0; i < ideologies.Count; i++)
                {
                    if (ideologies.ElementAt(i).Key != risingIdeology.Item1 && ideologies.ElementAt(i).Value != 0)
                    {
                        if (ideologies.ElementAt(i).Value - totalToDecrease > 0)
                        {
                            ideologies[ideologies.ElementAt(i).Key] -= totalToDecrease;
                        }
                        else
                        {
                            // add the difference to the totalToDecrease divided by the number of  the remaining non zeroe ideologies
                            double delta = totalToDecrease - ideologies.ElementAt(i).Value;
                            ideologies[ideologies.ElementAt(i).Key] = 0;
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
                ideologies[risingIdeology.Item1] = 1;
                // all other are at zero
                for (int i = 0; i < ideologies.Count; i++)
                {
                    if (ideologies.ElementAt(i).Key != risingIdeology.Item1)
                    {
                        ideologies[ideologies.ElementAt(i).Key] = 0;
                    }
                }
            }

            //print the Sum of all ideologies
            //Console.WriteLine(ideologies.Sum(x => x.Value));

        }

        //Internal Statistics
        private double productivity;
        public double Productivity { get => productivity; set { if (value > 1) { productivity = 1; } else if (value < 0) { productivity = 0; } else { productivity = value; } } }
        private double educationRate;
        public double EducationRate { get => educationRate; set  {if (value > 1) { educationRate = 1; } else if ( value < 0) { educationRate = 0; } else { educationRate = value; } } }
        private double healthRate;
        public double HealthRate { get => healthRate; set { if ( value > 1) { healthRate = 1; } else if ( value < 0) { healthRate = 0; } else { healthRate = value; } } }
        private double happinessRate;
        public double HappinessRate { get => happinessRate; set { if ( value > 1) { happinessRate = 1; } else if ( value < 0) { happinessRate = 0; } else { happinessRate = value; } } }
        private double corruptionRate;
        public double CorruptionRate { get => corruptionRate; set  {if ( value > 1) { corruptionRate = 1; } else if ( value < 0) { corruptionRate = 0; } else { corruptionRate = value; } } }
        private double crimeRate;
        public double CrimeRate { get => crimeRate; set { if ( value > 1) { crimeRate = 1; } else if ( value < 0) { crimeRate = 0; } else { crimeRate = value; } } }
        private double foodRate;
        public double FoodRate { get => foodRate; set { if ( value > 1) { foodRate = 1; } else if ( value < 0) { foodRate = 0; } else { foodRate = value; } } }

        private int industrialPower;
        public double IndustrialPower { get => industrialPower; set => industrialPower = (int)(value < 0 ? 0 : value); }
        private int agriculturalPower;
        public double AgriculturalPower { get => agriculturalPower; set => agriculturalPower = (int)(value < 0 ? 0 : value); }
        private int tertiaryPower;
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

        public PoliticalPlan CurrentPlan { get => currentPlan; set => currentPlan = value; }

        public void initInternalStatistics( int infrastructurePower, 
            double productivity, double educationRate, double healthRate,
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
            if (currentPlan.isFinished())
            {
                currentPlan.takeEffect(this);
                currentPlan = null;
                //TODO: change the percentage value :
                if (NbWarEngagedIn == 0)
                {
                    if (Random.NextDouble() < 0.5) // We pick a plan depending on the current ideology : 
                    {
                        switch (ideologies.Last().Key)
                        {
                            case Ideology.Communism:
                                currentPlan = (PoliticalPlan)CommunistPoliticalPlan[Random.Next(CommunistPoliticalPlan.Count)].Clone();
                                currentPlan.init();
                                break;

                            case Ideology.Socialism: 
                                currentPlan = (PoliticalPlan)SocialistPoliticalPlan[Random.Next(SocialistPoliticalPlan.Count)].Clone();
                                currentPlan.init();
                                break;

                            case Ideology.Despotism:
                                currentPlan = (PoliticalPlan)DespoticPlan[Random.Next(DespoticPlan.Count)].Clone();
                                currentPlan.init();
                                break;

                            case Ideology.Fascism: 
                                currentPlan  = (PoliticalPlan)FascistPoliticalPlan[Random.Next(FascistPoliticalPlan.Count)].Clone();
                                currentPlan.init();
                                break;

                            default:
                                currentPlan = (PoliticalPlan)AltRightPoliticalPlan[Random.Next(AltRightPoliticalPlan.Count)].Clone();
                                currentPlan.init();
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
        public decimal GDPGrowthRate { get => _gdpGrowthRate; set => _gdpGrowthRate = value < 0 ? 0 : value; }


        public decimal Expenses { get => expenses; set => expenses = value < 0 ? 0 : value; }
        public decimal Incomes { get => incomes; set => incomes = value < 0 ? 0 : value; }
        public decimal Debt { get => debt; set => debt = value < 0 ? 0 : value; }
        public decimal DebtInterest { get => debtInterest; set => debtInterest = value < 0 ? 0 : value; }
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
            incomes = ((decimal)((AgriculturalPower + IndustrialPower + TertiaryPower) * (double)Population))/700000;
            expenses = ((decimal)healthRate * Population * 5 + (decimal)educationRate * Population * 1 +
                        (decimal)foodRate * Population * 1)/10000 + (decimal)Military/10000;

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


        public int MilitaryPact { get; set; }


        private decimal military;
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

        public int VictoryPoints { get; set; }

        private int currentVictoryPoints;
        public int CurrentVictoryPoints
        {
            get => currentVictoryPoints;
            set => currentVictoryPoints = value < 0 ? 0 : value;
        }

        //Demographic Statistics
        private decimal _population;
        public decimal Population
        {
            get => _population;
            set
            {
                if (Population + value >= 0)
                {
                    _population = value;
                }
                else
                {
                    _population = 0;
                }


            }
        }

        public double PopulationGrowthRate { get; set; }
        public double PopulationDeathRate { get; set; }
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
