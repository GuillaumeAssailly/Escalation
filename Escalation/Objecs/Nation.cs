using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escalation
{

    //Enum for ideologies
    internal enum Ideology
    {
        Fascism,
        Communism,
        SocDemocracy,
        Monarchy,
        AuthLiberalism,
        Anarchy
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
        Republic, Kingdom, DemocraticRepublic, Duchy, Barony, County, Empire
    }


    internal class Nation
    {

        public Ecode Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Flag { get; set; }

        public ETitle Title { get; set; }

        public Nation() { }


        
        //Political Statistics
            private int politicalPower;
            private int stability;


        
            private Dictionary<Ideology, int> ideologies;       //Dictionnary of ideologies to their respective percentages




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
            private float populationDensity;
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
