using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Escalation.Utils;
using Escalation.World;
using Random = Escalation.Utils.Random;
namespace Escalation.Manager
{
    public  class RelationManager : Manager
    {
        public RelationManager(Earth World, Random Random) : base(World, Random)
        {

        }

        public void initAlliances()
        {
            World.Alliances.Add(new Alliance("NATO" ));
            World.Alliances.Add(new Alliance("Warsaw Pact"));

            ///NATO MEMBERS
            World.Alliances[0].AddMember(World.Nations[(int)Ecode.FRA]);
            World.Alliances[0].AddMember(World.Nations[(int)Ecode.ROY]);
            World.Alliances[0].AddMember(World.Nations[(int)Ecode.ESP]);
            World.Alliances[0].AddMember(World.Nations[(int)Ecode.POR]);
            World.Alliances[0].AddMember(World.Nations[(int)Ecode.ISL]);
            World.Alliances[0].AddMember(World.Nations[(int)Ecode.LUX]);
            World.Alliances[0].AddMember(World.Nations[(int)Ecode.BEL]);
            World.Alliances[0].AddMember(World.Nations[(int)Ecode.PAB]);
            World.Alliances[0].AddMember(World.Nations[(int)Ecode.GDR]);
            World.Alliances[0].AddMember(World.Nations[(int)Ecode.ITA]);
            World.Alliances[0].AddMember(World.Nations[(int)Ecode.DAN]);
            World.Alliances[0].AddMember(World.Nations[(int)Ecode.GRE]);
            World.Alliances[0].AddMember(World.Nations[(int)Ecode.TUR]);
            World.Alliances[0].AddMember(World.Nations[(int)Ecode.ETU]);
            World.Alliances[0].AddMember(World.Nations[(int)Ecode.CAN]);



            ///WARSAW PACT MEMBERS
            World.Alliances[1].AddMember(World.Nations[(int)Ecode.DDR]);
            World.Alliances[1].AddMember(World.Nations[(int)Ecode.POL]);
            World.Alliances[1].AddMember(World.Nations[(int)Ecode.TCQ]);
            World.Alliances[1].AddMember(World.Nations[(int)Ecode.HON]);
            World.Alliances[1].AddMember(World.Nations[(int)Ecode.ROM]);
            World.Alliances[1].AddMember(World.Nations[(int)Ecode.BUL]);
            World.Alliances[1].AddMember(World.Nations[(int)Ecode.RUS]);
            

        }

        public void initRelations()
        {
            World.RelationsMatrix = new int[World.Nations.Count, World.Nations.Count];
            World.WarMatrix = new bool[World.Nations.Count, World.Nations.Count];
            foreach (Nation nation in World.Nations)
            {
                foreach (Nation otherNation in World.Nations)
                {
                    if (nation != otherNation)
                    {
                        World.RelationsMatrix[(int)nation.Code, (int)otherNation.Code] = computeRelation(nation, otherNation);
                        World.WarMatrix[(int)nation.Code, (int)otherNation.Code] = false;
                    }
                    else
                    {
                        World.RelationsMatrix[(int)nation.Code, (int)otherNation.Code] = 100000; //Debug purpose
                        World.WarMatrix[(int)nation.Code, (int)otherNation.Code] = true; //very important
                    }
                }
            }
        }

        public void updateRelations(Nation n)
        {
            foreach (Nation nation in World.Nations)
            {
                if (nation != n)
                {
                    World.RelationsMatrix[(int)n.Code, (int)nation.Code] = computeRelation(n, nation);
                }
            }
        }

        private int computeRelation(Nation nation, Nation otherNation)
        {
            int relation = 0;
         
            //Compute the relation between the two nations :

            //We first take care of the main ideologies of each nation :

            //7 by 7 int matrix
            
            int[,]  ideologyMatrix = new int[7, 7] 
              { { 100, 70, 20, -10, -50, -50, -100 },
                { 70, 100, 50, 0, -20, -60,-80 },
                { 20, 50, 100, 20, -20, -60, -80 },
                { -10, 0, 20, 100, 50, 0, -30 },
                { -50, -20, -20, 50, 100, 40, 20 },
                { -50, -60, -60, 0, 40, 100, 50 },
                { -100, -80, -80, -30, 20, 50, 100 } };

            int ideologyRelation = ideologyMatrix[(int)nation.getIdeologies().Last().Key, (int)otherNation.getIdeologies().Last().Key];
            
            relation += ideologyRelation;

            //We then take care of the neighbors of each nation :
            if (nation.GetNeighbors().ContainsKey(otherNation.Code))
            {
                if (ideologyRelation > 0) //If the two nations have the same ideology, they are more likely to be friends
                {
                    relation += Random.Next(100);
                }
                else
                {
                    relation -= Random.Next(400); ;
                }
            }

            //It is important to see if the two nations are in the same alliance :
            if (nation.MilitaryPact != -1 && otherNation.MilitaryPact != -1)
            {
                if (nation.MilitaryPact == otherNation.MilitaryPact)
                {
                    relation += Random.Next(100,200);
                }
                else
                {
                    relation -= Random.Next(100,200);
                }
            }
            
            //We check if the two nations are at war with each other (to prevent joining any alliance between them)
            if (World.WarMatrix[(int)nation.Code, (int)otherNation.Code] == true)
            {
                relation -= Random.Next(200, 500);
            }
            


            //Some randomness to make it fun :)

            relation+= Random.Next(-100, 100);
            return relation;
        }


        public void GoToWar()
        {
            //We will create wars depending on the relations between nations and the world tension.
            // Two nations that are neighboors are more likely to go to war, we can as well have a world war if the world tension is high enough.

            if (Random.NextDouble() < World.WorldTension/200)
            {
                if (Random.NextDouble() < (double)World.Wars.Count/20) //We limit the max number of wars at roughly 20
                {
                    // We join a war !!!

                    //We select a random ongoing war where the wars.count > 0
                    var eligibleWars = World.Wars.Where(w => w.Attackers.Count > 0 && w.Defenders.Count > 0).ToList();
                    War war;
                    if (eligibleWars.Count > 0)
                    {
                        war = eligibleWars[Random.Next(eligibleWars.Count)];
                    }
                    else
                    {
                        return;
                    }
                    if (Random.NextDouble() < 0.5 )
                    {
                        // we then select a random attacker :
                        Nation attacker = war.Attackers[Random.Next(war.Attackers.Count)];

                        //We then select a random neighbor of him : 
                        Nation attackerNeighbor = World.Nations[(int)attacker.GetNeighbors().Keys.ElementAt(Random.Next(attacker.GetNeighbors().Count))];

                        //We then select a random defender :
                        Nation defender = war.Defenders[Random.Next(war.Defenders.Count)];

                        if (World.RelationsMatrix[(int)attackerNeighbor.Code, (int)defender.Code] < 100 && World.WarMatrix[(int)attackerNeighbor.Code, (int)defender.Code] == false)
                        {
                            if (war.Defenders.Any(n => n.MilitaryPact == attackerNeighbor.MilitaryPact &&
                                                       attackerNeighbor.MilitaryPact != -1) || war.Attackers.Contains(attackerNeighbor))
                            {
                                return;
                            }
                            war.AddAttacker(attackerNeighbor);
                            foreach (Nation n in war.Defenders)
                            {
                                World.WarMatrix[(int)n.Code, (int)attackerNeighbor.Code] = true;
                                World.WarMatrix[(int)attackerNeighbor.Code, (int)n.Code] = true;
                            }
                            World.WorldTension += (double)(attackerNeighbor.Military / 100);

                        }
                    }
                    else
                    {
                        // we then select a random defender :
                        Nation defender = war.Defenders[Random.Next(war.Defenders.Count)];

                        //We then select a random neighbor of him : 
                        Nation defenderNeighbor = World.Nations[(int)defender.GetNeighbors().Keys.ElementAt(Random.Next(defender.GetNeighbors().Count))];

                        //We then select a random attacker :
                        Nation attacker = war.Defenders[Random.Next(war.Defenders.Count)];

                        if (World.RelationsMatrix[(int)defenderNeighbor.Code, (int)defender.Code] < 100 && World.WarMatrix[(int)defenderNeighbor.Code, (int)defender.Code] == false)
                        {
                            if (war.Attackers.Any(n => n.MilitaryPact == defenderNeighbor.MilitaryPact &&
                                                       defenderNeighbor.MilitaryPact != -1) || war.Attackers.Contains(defenderNeighbor))
                            {
                                return;
                            }
                            war.AddDefender(defenderNeighbor);
                            foreach (Nation n in war.Attackers)
                            {
                                World.WarMatrix[(int)n.Code, (int)defenderNeighbor.Code] = true;
                                World.WarMatrix[(int)defenderNeighbor.Code, (int)n.Code] = true;
                            }
                            
                            World.WorldTension += (double)(defenderNeighbor.Military / 100);

                        }
                    }
                   
                }
                else // We create a War !!
                {
                    Nation attacker = World.Nations[Random.Next(World.Nations.Count)];
                    Nation defender = attacker;
                    if (Random.NextDouble() < 0.05) //Interventions war : basically two randoms nations that may not have any links declare wars on each other
                    {
                        //We check for two nations that are not allies and that are not at war with each other :
                        while ((attacker.MilitaryPact!=-1&&defender.MilitaryPact==attacker.MilitaryPact) || defender == attacker || World.WarMatrix[(int)attacker.Code, (int)defender.Code] == true)
                        {
                            defender = World.Nations[Random.Next(World.Nations.Count)];
                        }
                    }
                    else if (World.Nations[(int)attacker.Code].GetNeighbors().Count() > 1) //Neighbor conflict : two neighboors nations declare war on each other
                    {
                        //We have to check if the attacker still has neighboors that are not at war with him :
                        bool isOk = World.Nations[(int)attacker.Code].GetNeighbors().Keys.Any(n => World.WarMatrix[(int)attacker.Code, (int)n] == false && World.Nations[(int)attacker.Code].MilitaryPact != World.Nations[(int)n].MilitaryPact);
                        //TODO : Check if neighbooors are not all parts of the same Military Pact !
                        bool isOk2 = World.Nations[(int)attacker.Code].GetNeighbors().Keys.Any(n => World.Nations[(int)n].MilitaryPact != attacker.MilitaryPact);
                        
                        if (isOk && isOk2)
                        {
                            while ((attacker.MilitaryPact!=-1&&defender.MilitaryPact==attacker.MilitaryPact) || defender == attacker || World.WarMatrix[(int)attacker.Code, (int)defender.Code] == true)
                            {
                                defender = World.Nations[(int)World.Nations[(int)attacker.Code].GetNeighbors().Keys.ElementAt(Random.Next(World.Nations[(int)attacker.Code].GetNeighbors().Count))];
                            }

                        }
                        else
                        {
                            return;
                        }

                    }
                    else
                    {
                        return;
                    }
                    if (World.RelationsMatrix[(int)attacker.Code, (int)defender.Code] < 0
                        && (World.WorldTension >= 60 ||  defender.MilitaryPact == -1))
                    {
                        
                        //THEN THE WAR ERUPTS !
                        War w = new War(new List<Nation>() { attacker }, new List<Nation>() { defender },
                            World.CurrentDate);
                        w.AnnexationOccurred += Annex;
                        World.Wars.Add(w);

                        World.WarMatrix[(int)attacker.Code, (int)defender.Code] = true;
                        World.WarMatrix[(int)defender.Code, (int)attacker.Code] = true;

                        World.WorldTension += 2* (double)((attacker.Military + defender.Military) / 100);



                        //We eventually add some allies to the war :
                        // We first check if DefCOn is at 2 or less :
                        
                        if (World.WorldTension >= 60 &&  attacker.MilitaryPact != -1)
                        {
                            foreach (var n in World.Alliances[attacker.MilitaryPact].GetMembers().Where(n => n != attacker && n != defender &&
                                         World.RelationsMatrix[(int)n.Code, (int)defender.Code] < 100 && World.WarMatrix[(int)n.Code, (int)defender.Code] == false && n.MilitaryPact!=defender.MilitaryPact))
                            {
                                World.Wars.Last().AddAttacker(n);
                                World.WarMatrix[(int)n.Code, (int)defender.Code] = true;
                                World.WarMatrix[(int)defender.Code, (int)n.Code] = true;
                                World.WorldTension += (double)(n.Military / 100);
                            }
                        }

                        if (World.WorldTension >= 60 && defender.MilitaryPact != -1)
                        {
                            foreach (var n in World.Alliances[defender.MilitaryPact].GetMembers().Where(n => n != attacker && n != defender &&
                                         World.RelationsMatrix[(int)n.Code, (int)attacker.Code] < 100 && World.WarMatrix[(int)n.Code, (int)attacker.Code] == false &&n.MilitaryPact!=attacker.MilitaryPact))
                            {
                                World.Wars.Last().AddDefender(n);
                                World.WarMatrix[(int)n.Code, (int)attacker.Code] = true;
                                World.WarMatrix[(int)attacker.Code, (int)n.Code] = true;
                                World.WorldTension += (double)(n.Military / 100);
                            }
                        }
                    }
                }
                


            }

        }


        public void ManageWars()
        {
            List<War> warsToRemove = new List<War>();
            foreach (War w in World.Wars)
            {
                if(w.DayPassed())
                {
                    warsToRemove.Add(w);
                }
            }
            foreach (War w in warsToRemove)
            {
                foreach (Nation n in w.Attackers)
                {
                    foreach (Nation n2 in w.Defenders)
                    {
                        World.WarMatrix[(int)n.Code, (int)n2.Code] = false;
                        World.WarMatrix[(int)n2.Code, (int)n.Code] = false;
                    }
                }
                w.Attackers.ForEach(nation=> nation.NbWarEngagedIn--);
                w.Defenders.ForEach(nation => nation.NbWarEngagedIn--);

                World.WorldTension -= (double)w.DaysElapsed / 100;
                World.Wars.Remove(w);

            }


        }

        public void ManageTension()
        {
            World.WorldTension  =(World.WorldTension -  0.0001 * World.WorldTension) + 0.001 * World.Wars.Count;
        }

        public void ManageAlliances()
        {

            if (Random.NextDouble() < World.WorldTension/100)
            {
                if (World.WorldTension >= 60) // We are at Defcon 2, meaning no more great alliances
                                              // will be created, and nations will rather choose to join existing alliances
                {
                    //We select a random world country : 
                    Nation n = World.Nations[Random.Next(World.Nations.Count)];
                    //We select a random alliance :
                    if (n.MilitaryPact == -1 )
                    {
                        Alliance a = World.Alliances[Random.Next(World.Alliances.Count)];
                        while (!a.GetMembers().Any())
                        {
                            a = World.Alliances[Random.Next(World.Alliances.Count)];
                        }
                        if (World.RelationsMatrix[(int)a.GetMembers().First().Code, (int)n.Code] > 100 )
                        {
                            foreach (Nation n2 in a.GetMembers())
                            {
                                foreach (War w in World.Wars)
                                {
                                    //We check if the two nations are at war with each other :
                                    if ((w.Attackers.Contains(n2) && w.Defenders.Contains(n)) || (w.Attackers.Contains(n) && w.Defenders.Contains(n2)))
                                    {
                                        return;
                                    }
                                }
                            }
                            a.AddMember(n);
                            Trace.WriteLine(n.Code + " has joined " + a.GetMembers().First().Code +"'s alliance" );
                        }
                    }
                }
                else
                {
                    //We create an alliance between two nations that are not at war with each other and that are not allies :


                    //We select a random world country :
                    Nation n = World.Nations[Random.Next(World.Nations.Count)];
                    if (n.MilitaryPact == -1)
                    {
                        Nation n2 = World.Nations[(int)n.GetNeighbors().Keys.ElementAt(Random.Next(n.GetNeighbors().Count))];
                        if (n2.MilitaryPact==-1 && n!=n2){
                            foreach (War w in World.Wars)
                            {
                                if ((w.Attackers.Contains(n) && w.Defenders.Contains(n2))|| (w.Attackers.Contains(n2) &&
                                        w.Defenders.Contains(n2)))
                                {
                                    return;
                                }
                            }
                        }
                        else
                        {
                            return;
                        }

                        Alliance a;
                        if (World.RelationsMatrix[(int)n.Code, (int)n2.Code] > 250)
                        {
                            a = new Alliance(n.Code + "-" + n2.Code + " pact");
                            a.AddMember(n);
                            a.AddMember(n2);
                            World.Alliances.Add(a);
                            Trace.WriteLine(a.GetMembers().First().Code + " has created an alliance with " +
                                            a.GetMembers().Last().Code);
                        }
                        else
                        {
                            return;

                        }
                        foreach (Ecode neighborN in n.GetNeighbors().Keys)
                        {
                            if (World.Nations[(int)neighborN].MilitaryPact == -1 && !a.GetMembers().Contains(World.Nations[(int)neighborN]))
                            {
                                if (World.RelationsMatrix[(int)a.GetMembers().First().Code, (int)n.Code] > 100)
                                {
                                    foreach (Nation members in a.GetMembers())
                                    {
                                        foreach (War w in World.Wars)
                                        {
                                            //We check if the two nations are at war with each other :
                                            if ((w.Attackers.Contains(members) && w.Defenders.Contains(n)) ||
                                                (w.Attackers.Contains(n) && w.Defenders.Contains(members)))
                                            {
                                                return;
                                            }
                                        }
                                    }
                                    a.AddMember(World.Nations[(int)neighborN]);
                                    Trace.WriteLine(n.Code + " has joined " + a.GetMembers().First().Code + "'s alliance");
                                }
                            }
                        }
                    }
                }
            }


        }

        public void Annex(object sender, AnnexionNationEventArgs e)
        {
            War w = (War)sender;

              foreach (War war in World.Wars)
            {
                if (war.Attackers.Contains(e.AnnexedNation))
                {
                    war.DisengageAttacker(e.AnnexedNation);
                    foreach (Nation n in war.Defenders)
                    {
                        World.WarMatrix[(int)n.Code, (int)e.AnnexedNation.Code] = false;
                        World.WarMatrix[(int)e.AnnexedNation.Code, (int)n.Code] = false;
                    }
                }

                if (war.Defenders.Contains(e.AnnexedNation))
                {
                     war.DisengageDefender(e.AnnexedNation);
                    foreach (Nation n in war.Attackers)
                    {
                        World.WarMatrix[(int)n.Code, (int)e.AnnexedNation.Code] = false;
                        World.WarMatrix[(int)e.AnnexedNation.Code, (int)n.Code] = false;
                    }
                }
                
            }


            if (e.IsDefender)
            {
                Nation strongestAttacker = w.Attackers.OrderByDescending(n => n.Military).FirstOrDefault();
                if (e.AnnexedNation.MilitaryPact != -1)
                {
                    World.Alliances[e.AnnexedNation.MilitaryPact].RemoveMember(e.AnnexedNation);
                }


                if (strongestAttacker.MilitaryPact != -1)
                {
                    //We stop the wars between every members
                    foreach (Nation n in World.Alliances[strongestAttacker.MilitaryPact].GetMembers())
                    {
                        foreach (War war in World.Wars)
                        {
                            if (war.Attackers.Contains(n) && war.Defenders.Contains(e.AnnexedNation))
                            {
                                war.DisengageDefender(e.AnnexedNation);
                                World.WarMatrix[(int)n.Code, (int)e.AnnexedNation.Code] = false;
                                World.WarMatrix[(int)e.AnnexedNation.Code, (int)n.Code] = false;
                            }

                            if (war.Attackers.Contains(e.AnnexedNation) && war.Defenders.Contains(n))
                            {
                                war.DisengageDefender(e.AnnexedNation);
                                World.WarMatrix[(int)n.Code, (int)e.AnnexedNation.Code] = false;
                                World.WarMatrix[(int)e.AnnexedNation.Code, (int)n.Code] = false;
                            }
                         
                        }
                    }
                   
                  
                    World.Alliances[strongestAttacker.MilitaryPact].AddMember(e.AnnexedNation);
                }
                e.AnnexedNation.SetIdeologies(strongestAttacker.getIdeologies());
                e.AnnexedNation.CurrentVictoryPoints = e.AnnexedNation.VictoryPoints;
                World.WarMatrix[(int)strongestAttacker.Code, (int)e.AnnexedNation.Code] = false;
                World.WarMatrix[(int)e.AnnexedNation.Code, (int)strongestAttacker.Code] = false;

                Trace.WriteLine(w.Name + strongestAttacker.Code + " has seized the territory of " + e.AnnexedNation.Code);
            }
            else
            {
                Nation strongestDefender = w.Defenders.OrderByDescending(n => n.Military).FirstOrDefault();
                if (e.AnnexedNation.MilitaryPact != -1)
                {
                    World.Alliances[e.AnnexedNation.MilitaryPact].RemoveMember(e.AnnexedNation);
                }


                if (strongestDefender.MilitaryPact != -1)
                {
                    //We stop the wars between every members
                    foreach (Nation n in World.Alliances[strongestDefender.MilitaryPact].GetMembers())
                    {
                        foreach (War war in World.Wars)
                        {
                            if (war.Attackers.Contains(n) && war.Defenders.Contains(e.AnnexedNation))
                            {
                                war.DisengageDefender(e.AnnexedNation);
                                World.WarMatrix[(int)n.Code, (int)e.AnnexedNation.Code] = false;
                                World.WarMatrix[(int)e.AnnexedNation.Code, (int)n.Code] = false;
                            }

                            if (war.Attackers.Contains(e.AnnexedNation) && war.Defenders.Contains(n))
                            {
                                war.DisengageDefender(e.AnnexedNation);
                                World.WarMatrix[(int)n.Code, (int)e.AnnexedNation.Code] = false;
                                World.WarMatrix[(int)e.AnnexedNation.Code, (int)n.Code] = false;
                            }

                        }
                    }
                    World.Alliances[strongestDefender.MilitaryPact].AddMember(e.AnnexedNation);
                }

                e.AnnexedNation.SetIdeologies(strongestDefender.getIdeologies());
                e.AnnexedNation.CurrentVictoryPoints = e.AnnexedNation.VictoryPoints;
                World.WarMatrix[(int)strongestDefender.Code, (int)e.AnnexedNation.Code] = false;
                World.WarMatrix[(int)e.AnnexedNation.Code, (int)strongestDefender.Code] = false;

                Trace.WriteLine(w.Name + strongestDefender.Code + " has seized the territory of " + e.AnnexedNation.Code);
            }

        }

      
    }
  
}
