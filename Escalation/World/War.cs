using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Random = Escalation.Utils.Random;

namespace Escalation.World
{
    public class War 
    {



        public string Name;


        public List<Nation> Attackers { get; private set; }
        public List<Nation> Defenders { get; private set; }


        public DateTime StartDate;

        private int daysElapsed;
        private int timeToLive;

        private double probabilityOfEndingConflict;



        public int DaysElapsed
        {
            get => daysElapsed;
            set
            {
                if (value < 0)
                {
                    daysElapsed = 0;
                }
                else
                {
                    daysElapsed = value;
                }
            }
        }

        private int score;

        private int scoreMin;
        private int scoreMax;


        public bool DayPassed()
        {
            bool res = false;
            timeToLive--;
            computeCasualties();   

            if (Random.NextDouble() < 0.05)
            {
                computeBattle();
            }

            if (score == scoreMin) //the defenders win !
            {
                Console.WriteLine(Name + "The defenders win !");
                res = true;
            }
            else if (score == scoreMax) 
            {
                Console.WriteLine(Name+"The attackers win !");
                res = true;
            }

            if (daysElapsed > timeToLive)
            {
                if (Random.NextDouble() < probabilityOfEndingConflict)
                {
                    if (score > 0) // the attackers win !
                    {
                        Console.WriteLine(Name+"The attackers win !");
                        res = true;
                    }
                    else // the defenders win !
                    {
                        Console.WriteLine(Name+"The defenders win !");
                        res = true;
                    }
                }
                else
                {
                    probabilityOfEndingConflict *= 2;
                }
            }

            return res;

        }

        private void computeBattle()
        {

            //We select in which nation the battle will take place :
            Nation attacker = Attackers[Random.Next(0, Attackers.Count)];

            Nation defender = Defenders[Random.Next(0, Defenders.Count)];

            int attackersPoint= 0, defendersPoint = 0, totalMilitaryPoints = 0;

            foreach (Nation n in Attackers)
            {
                attackersPoint += n.Military;
            }

            foreach (Nation n in Defenders)
            {
                defendersPoint -= n.Military;
            }

            totalMilitaryPoints = attackersPoint - defendersPoint +  Random.Next(-defendersPoint, attackersPoint);

            if (totalMilitaryPoints > 0) // Attacker wins the day !
            {
                score++;
                defender.CurrentVictoryPoints--;
                Console.WriteLine(this.Name + attacker.Code + " wins the day !");
            }
            else //Defender wins the day !
            {
                score--;
                defender.CurrentVictoryPoints++;
                Console.WriteLine(defender.Code + " wins the day !");

            }

            
        }

        private void computeCasualties()
        {
            Nation attacker = Attackers[Random.Next(0, Attackers.Count)];
            Nation defender = Defenders[Random.Next(0, Defenders.Count)];

            attacker.Population -= Random.Next(0, (ulong)attacker.Population/10000);
            defender.Population -= Random.Next(0, (ulong)defender.Population/10000);
            attacker.Military -= (int)Random.Next(0, (ulong)attacker.Military/10000);
            defender.Military -= (int)Random.Next(0, (ulong)defender.Military/10000);
        }


        public War(List<Nation> attackers, List<Nation> defenders, DateTime startDate)
        {
            Name = attackers.First().Code + " VS " + defenders.First().Code + ":";
            Attackers = attackers;
            Defenders = defenders;
            StartDate = startDate;
            score = 0;
            scoreMin = 0;
            scoreMax = 0;
            probabilityOfEndingConflict = 0.1;

            foreach(Nation n in defenders)
            {
                scoreMax += n.VictoryPoints;
            }


            foreach (Nation n in attackers)
            {
                scoreMin -= n.VictoryPoints;
            }
            
            timeToLive = Math.Abs(scoreMin)+scoreMax * Random.Next(100);

            Console.WriteLine("New War on " + StartDate + " between " + attackers.First().Code + "(attacker) and " + defenders.First().Code +" (defender)");
            Console.WriteLine("Time to live : " + timeToLive);
            Console.WriteLine("Score of defender : " + scoreMax);
            Console.WriteLine("Score of attacker : " + scoreMin);
        }

        public void AddAttacker(Nation n)
        {
            Attackers.Add(n);
            scoreMin += n.VictoryPoints;
        }

        public void AddDefender(Nation n)
        {
            Defenders.Add(n);
            scoreMax -= n.VictoryPoints;
        }


    }
}
