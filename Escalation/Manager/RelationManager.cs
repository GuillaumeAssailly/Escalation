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

        public void initRelations()
        {
            World.RelationsMatrix = new int[World.Nations.Count, World.Nations.Count];
            foreach (Nation nation in World.Nations)
            {
                foreach (Nation otherNation in World.Nations)
                {
                    if (nation != otherNation)
                    {
                        World.RelationsMatrix[(int)nation.Code, (int)otherNation.Code] = computeRelation(nation, otherNation);
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
            
            //Some randomness to make it fun :)

            relation+= Random.Next(-100, 100);
            return relation;
        }


        public void GoToWar()
        {
            //We will create wars depending on the relations between nations and the world tension.
            // Two nations that are neighboors are more likely to go to war, we can as well have a world war if the world tension is high enough.



        }


        public void CreateAlliances()
        {
            // We will define different types of Alliance to be created !
            // Do remember that alliance are more likely to be created when the world tension is high !!!


        }


    }
  
}
