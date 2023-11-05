using Escalation.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        AFG, SAH, RSA, ALB, ALG, ALL, AND, ANG, ASA, ARG, ARM, AUS, PAL,
        AUT, AZE, BAH, BAN, BEL, BEI, BEN, BHO, BIE, BIR, BOL, BOS, BOT,
        BRA, BRU, BUL, BUR, BUD, CAM, CAO, CAN, CHL, CHI, CHY, COL, NCO,
        SCO, COS, CIV, CRO, CUB, DAN, DJI, EGY, EAU, EQA, ERY, ESP, EST,
        ESW, ETU, ETH, FID, FIN, FRA, GAB, GAM, GEO, GHA, GRE, GRO, GUA,
        GUI, GEQ, GBI, GUY, FGY, HAI, HOD, HON, IMA, ISA, IND, IDO, IRK,
        IRA, IRL, ISL, ISR, ITA, JAM, JAP, JOR, KAZ, KEN, KIR, KOS, KOW,
        LAO, LES, LET, LIB, LIA, LIY, LIT, LUX, MAC, MAD, MAS, MAW, MAL,
        MAR, MAU, MEX, MOL, MON, MOG, MOT, MOZ, NAM, NEP, NIC, NGR, NIG,
        NOR, NCA, NZE, OMA, OGD, OUZ, PAK, PAN, PNG, PAR, PAB, PER, PHI,
        POL, PRI, POR, QAT, RCE, RDE, RDO, RDC, ROM, ROY, RUS, RWA, SAL,
        SEN, SER, SLE, SLV, SLO, SOM, SOU, SDS, SRI, SUE, SUI, SUR, SVA,
        SYR, TAD, TAW, TAN, TCH, TCQ, TAA, THA, TOR, TOG, TET, TUN, TUM,
        TUR, UKR, URU, VAN, VAT, VEN, VIE, YEM, ZAM, ZIM,
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

        public string Name { get; set; }

        public string Description { get; set; }

        public string Flag { get; set; }

        public ETitle Title { get; set; }

        //List of Neighbors with qualifiers (M for Marine, L for Land): 
        private Dictionary<Ecode, char> neighbors;


        public Nation(Ecode code,
            double stability, int politicalPower, double Communism, double Socialism, double LeftWingDemocracy, double RightWingDemocracy, double Authoritarianism, double Despotism, double Fascism,
            decimal population, double populationGrowthRate, double populationDeathRate, double populationDensity)
        {
            Code = code;
            Stability = stability;
            this.politicalPower = politicalPower;
            SetIdeologies(Communism, Socialism, LeftWingDemocracy, RightWingDemocracy, Authoritarianism, Despotism, Fascism);

            Population = population;
            PopulationGrowthRate = populationGrowthRate;
            PopulationDeathRate = populationDeathRate;
            PopulationDensity = populationDensity;
            PopulationHistory = new List<decimal>();

            TreasuryHistory = new List<decimal>();
            IncomesHistory = new List<decimal>();
            ExpensesHistory = new List<decimal>();
            DebtHistory = new List<decimal>();

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
        private int politicalPower;
        public double Stability { get; set; }



        private Dictionary<Ideology, double> ideologies;       //Dictionnary of ideologies to their respective percentages
        private Tuple<Ideology, double> risingIdeology;        //Tuple of the ideology that is rising and the percentage gained every day


        public void SetRisingIdeology(Ideology ideology, double percentage)
        {
            risingIdeology = new Tuple<Ideology, double>(ideology, percentage);
        }

        public Dictionary<Ideology, double> getIdeologies()
        {
            return ideologies;
        }

        public void SetIdeologies(double communism, double socialism, double leftWingDemocracy, double rightWingDemocracy, double authoritarianism, double despotism, double fascism)
        {
            ideologies = new Dictionary<Ideology, double>
                {
                    { Ideology.Communism, communism },
                    { Ideology.Socialism,socialism },
                    { Ideology.LeftWingDemocracy, leftWingDemocracy },
                    { Ideology.RightWingDemocracy, rightWingDemocracy },
                    { Ideology.Authoritarianism, authoritarianism },
                    { Ideology.Despotism, despotism },
                    { Ideology.Fascism, fascism }
                };
            //TODO : REPLACE RANDOM IDEOLOGIES GENERATOR : 
            
            double sum = 0;
            double[] numbers = new double[7];

            for (int i = 0; i < 6; i++)
            {
                double randomNumber = Random.NextDouble() *(1 - sum);
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
                    Console.WriteLine(ideologies.ElementAt(i).Key + " : " + ideologies.ElementAt(i).Value);

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




        //Economic Statistics
        private decimal expenses;
        private decimal incomes;
        private decimal debt;
        private decimal debtInterest;
        private decimal treasury;

        private decimal GDP;
        private decimal GDPGrowthRate;

        public decimal Expenses { get => expenses; set => expenses = value < 0 ? 0 : value; }
        public decimal Incomes { get => incomes; set => incomes = value < 0 ? 0 : value; }
        public decimal Debt { get => debt; set => debt = value < 0 ? 0 : value; }
        public decimal DebtInterest { get => debtInterest; set => debtInterest = value < 0 ? 0 : value; }
        public decimal Treasury { get => treasury; set => treasury = value < 0 ? 0 : value; }

        public void  initEconomicStats(decimal expenses, decimal incomes, decimal debt, decimal debtInterest, decimal treasury)
        {
            this.expenses = expenses;
            this.incomes = incomes;
            this.debt = debt;
            this.debtInterest = debtInterest;
            this.treasury = treasury;
        }

        public void UpdateTreasury()
        {
            
            decimal netBalance = incomes - expenses - debtInterest;

            treasury += netBalance;

            if (treasury <= 0)
            {
                debt += (decimal)Math.Abs(treasury); 
                debtInterest+= debt * 0.00001m;
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
                    debt += (decimal)Math.Abs(debt - treasury);
                    debtInterest += debt * 0.00001m;
                    treasury = 0;
                }
            }
            
            // Display all the values :
            Trace.WriteLine("Treasury : " + treasury + " Debt : " + debt + " Debt Interest : " + debtInterest + " Net Balance : " + netBalance);
        }




        //Military Statistics
        private double manpower;                            //Percentage of population currently in the military

        private int victoryPoints;

        private ulong landForces;
        private ulong airForces;
        private ulong navalForces;
        private ulong nuclearForces;




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

        public List<decimal> PopulationHistory { get; set; }
        public List<decimal> TreasuryHistory { get; set; }
        public List<decimal> IncomesHistory { get; set; }
        public List<decimal> ExpensesHistory { get; set; }
        public List<decimal> DebtHistory { get; set; }

        //Geographic Statistics
        private ulong landArea;
        private string maincity;


        //Intelligence Statistics
        private int espionagePoints;
        private int counterEspionagePoints;





    }
}
