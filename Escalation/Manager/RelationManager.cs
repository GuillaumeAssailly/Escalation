using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            World.Alliances.Add(new Alliance("NATO", "Blue"));
            World.Alliances.Add(new Alliance("Warsaw Pact", "Red"));

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
            
            //Some randomness to make it fun :)

            relation+= Random.Next(-100, 100);
            return relation;
        }


        public void GoToWar()
        {
            //We will create wars depending on the relations between nations and the world tension.
            // Two nations that are neighboors are more likely to go to war, we can as well have a world war if the world tension is high enough.

            if (Random.NextDouble() < World.WorldTension/100)
            {
                Nation attacker = World.Nations[Random.Next(World.Nations.Count)];
                Nation defender = attacker;
                if (Random.NextDouble() < 0.01) //Interventions war : basically two randoms nations that may not have any links declare wars on each other
                {
                    //We check for two nations that are not allies and that are not at war with each other :
                    while (defender == attacker || World.WarMatrix[(int)attacker.Code,(int)defender.Code] == true)
                    {
                        defender = World.Nations[Random.Next(World.Nations.Count)];
                    }
                }
                else if (World.Nations[(int) attacker.Code].GetNeighbors().Count()>1) //Neighbor conflict : two neighboors nations declare war on each other
                {
                    //We have to check if the attacker still has neighboors that are not at war with him :
                    bool isOk = false;
                    foreach (Ecode n in World.Nations[(int)attacker.Code].GetNeighbors().Keys)
                    {
                        if (World.WarMatrix[(int)attacker.Code, (int)n] == false)
                        {
                            
                            isOk = true;
                            break;
                        }
                    }
                    
                    if (isOk)
                    {
                        while (defender == attacker || World.WarMatrix[(int)attacker.Code, (int)defender.Code] == true)
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
                if (World.RelationsMatrix[(int)attacker.Code, (int)defender.Code] < 50 )
                {
                    //THEN THE WAR ERUPTS !
                    World.Wars.Add(new War(new List<Nation>() { attacker }, new List<Nation>() { defender }, World.CurrentDate));

                    World.WarMatrix[(int)attacker.Code, (int)defender.Code] = true;
                    World.WarMatrix[(int)defender.Code, (int)attacker.Code] = true;

                    World.WorldTension +=  (double)((attacker.Military + defender.Military)/100);



                    //We eventually add some allies to the war :
                    if (attacker.MilitaryPact != -1)
                    {
                        foreach (Nation n in World.Alliances[attacker.MilitaryPact].GetMembers())
                        {
                            if (n != attacker && n != defender &&
                                World.RelationsMatrix[(int)n.Code, (int)defender.Code] < 100 && World.WarMatrix[(int)n.Code,(int)defender.Code] == false)
                            {
                                World.Wars.Last().AddAttacker(n);
                                World.WarMatrix[(int)n.Code, (int)defender.Code] = true;
                                World.WarMatrix[(int)defender.Code, (int)n.Code] = true;
                            }
                        }
                    }

                    if (defender.MilitaryPact != -1)
                    {
                        foreach (Nation n in World.Alliances[defender.MilitaryPact].GetMembers())
                        {
                            if (n != attacker && n != defender &&
                                World.RelationsMatrix[(int)n.Code, (int)attacker.Code] < 100 && World.WarMatrix[(int)n.Code, (int)attacker.Code] == false)
                            {
                                World.Wars.Last().AddDefender(n);
                                World.WarMatrix[(int)n.Code, (int)attacker.Code] = true;
                                World.WarMatrix[(int)attacker.Code, (int)n.Code] = true;
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
                World.Wars.Remove(w);

            }


        }

        public void CreateAlliances()
        {
            // We will define different types of Alliance to be created !
            // Do remember that alliance are more likely to be created when the world tension is high !!!


        }


    }
  
}
