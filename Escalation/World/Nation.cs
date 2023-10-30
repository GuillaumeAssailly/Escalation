using System;
using System.Collections.Generic;
using System.Linq;

namespace Escalation.World
{

    //Enum for ideologies
    internal enum Ideology
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
    internal enum Ecode
    {
        AFG, ALB, ALG, ALL, AND, ANG, ARG, ARM, ASA, AUS, AUT, AZE, BAH, BAN, BEI, BEL, BEN, BHO, BIE, BIR,
        BOL, BOS, BOT, BRA, BRU, BUD, BUL, BUR, CAM, CAN, CAO, CHI, CHL, CHY, CIV, COL, COS, CRO, CUB, DAN, 
        DJI, EAU, EGY, EQA, ERY, ESP, EST, ESW, ETH, ETU, FGY, FID, FIN, FRA, GAB, GAM, GBI, GEO, GEQ, GHA, 
        GRE, GRO, GUA, GUI, GUY, HAI, HOD, HON, IDO, IMA, IND, IRA, IRK, IRL, ISA, ISL, ISR, ITA, JAM, JAP, 
        JOR, KAZ, KEN, KIR, KOS, KOW, LAO, LES, LET, LIA, LIB, LIY, LIT, LUX, MAC, MAD, MAL, MAR, MAS, MAU, 
        MAW, MEX, MOG, MOL, MON, MOT, MOZ, NAM, NCA, NCO, NEP, NGR, NIC, NIG, NOR, NZE, OGD, OMA, OUZ, PAB, 
        PAK, PAL, PAN, PAR, PER, PHI, PNG, POL, POR, PRI, QAT, RCE, RDC, RDE, RDO, ROM, ROY, RSA, RUS, RWA, 
        SAH, SAL, SCO, SDS, SEN, SER, SLE, SLO, SLV, SOM, SOU, SRI, SUE, SUI, SUR, SVA, SYR, TAA, TAD, TAN, 
        TAW, TCH, TCQ, TET, THA, TOG, TOR, TUM, TUN, TUR, UKR, URU, VAN, VAT, VEN, VIE, YEM, ZAM, ZIM

    }

    //Enum for Nation Title and type (duchy, etc.)
    internal enum ETitle
    {
        Republic, Kingdom, DemocraticRepublic, Duchy, Barony, County, Empire, Commune, SovietRepublic
    }


    internal class Nation
    {

        public Ecode Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Flag { get; set; }

        public ETitle Title { get; set; }

        public Nation(Ecode code, 
            double stability, int politicalPower, double Communism, double Socialism, double LeftWingDemocracy, double RightWingDemocracy, double Authoritarianism, double Despotism, double Fascism) 
        
        {
            this.Code = code;
            this.stability = stability;
            this.politicalPower = politicalPower;
            SetIdeologies(Communism, Socialism, LeftWingDemocracy, RightWingDemocracy, Authoritarianism, Despotism, Fascism);
            
            
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
        }


        //Political Statistics
            private int politicalPower;
            private double stability;

             
            
            private Dictionary<Ideology, double> ideologies;       //Dictionnary of ideologies to their respective percentages
            private Tuple<Ideology, double> risingIdeology;        //Tuple of the ideology that is rising and the percentage gained every day

            
            public void SetRisingIdeology(Ideology ideology, double percentage)
            {
                risingIdeology = new Tuple<Ideology, double>(ideology, percentage);
            }
        
        public void DriftIdeologies()
        {

            //sort ideologies by ascending percentage


            //check if rising ideology progression dont go over 1 :
            if (ideologies[risingIdeology.Item1] + risingIdeology.Item2 <= 1)
            {

                ideologies[risingIdeology.Item1] += risingIdeology.Item2;

                int nbIdeoNotNull = ideologies.Count(x =>x.Key!= risingIdeology.Item1 && x.Value > 0);

                double totalToDecrease = risingIdeology.Item2 / nbIdeoNotNull;

                ideologies = ideologies.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);


                for (int i = 0; i < ideologies.Count; i++)
                {
                    if (ideologies.ElementAt(i).Key != risingIdeology.Item1 && ideologies.ElementAt(i).Value != 0)
                    {
                        if (ideologies.ElementAt(i).Value - totalToDecrease > 0 )
                        {
                            
                            

                            ideologies[ideologies.ElementAt(i).Key] -= totalToDecrease; 

                        }
                        else
                        {
                            // add the difference to the totalToDecrease divided by the number of  the remaining non zeroe ideologies
                            double delta = totalToDecrease - ideologies.ElementAt(i).Value;
                            ideologies[ideologies.ElementAt(i).Key] = 0;
                            nbIdeoNotNull--;
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
            Console.WriteLine("Sum of all ideologies : " + (ideologies.Sum(x => x.Value)));


        }




        //Economic Statistics
        private ulong industrialCapacity;
            private ulong agriculturalCapacity;
            private ulong commercialCapacity;

            
            private ulong  GDP;  
            private double GDPgrowthRate;



        //Military Statistics
            private double manpower;                            //Percentage of population currently in the military

            private int victoryPoints;

            private ulong landForces; 
            private ulong airForces;
            private ulong navalForces;  
            private ulong nuclearForces;




        //Demographic Statistics
            private ulong population;
            private double populationGrowthRate;
            private double populationDensity;
            private double lifeExpectancy;
            private double povertyRate;





        //Geographic Statistics
            private ulong landArea;
            private string maincity;


        //Intelligence Statistics
            private int espionagePoints;
            private int counterEspionagePoints;





    }
}
