using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Random = Escalation.Utils.Random;

namespace Escalation.World
{

    public class AnnexionNationEventArgs : EventArgs
    {
        public Nation AnnexedNation { get; }
        public bool IsDefender { get; }

        public AnnexionNationEventArgs(Nation annexedNation, bool isDefender)
        {
            AnnexedNation = annexedNation;
            IsDefender = isDefender;
        }
    }
    public class War 
    {



        public string Name;

        public event EventHandler<AnnexionNationEventArgs> AnnexationOccurred;

        [JsonIgnore]
        public List<Nation> Attackers { get; private set; }

        [JsonIgnore]
        public List<Nation> Defenders { get; private set; }

        [JsonPropertyName("A")]
        public List<Ecode> CodeAttackers { get; private set; }

        [JsonPropertyName("D")]
        public List<Ecode> CodeDefenders { get; private set; }

        [JsonPropertyName("S")]
        public DateTime StartDate { get; set; }

        private int daysElapsed;
        private int timeToLive;
        private int numberOfCountries;


        private double probabilityOfEndingConflict;


        private int scoreDef;
        private int scoreAtt;

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

        
       


        public bool DayPassed()
        {
            bool res = false;
            timeToLive--;
            DaysElapsed++;

            if (Attackers.Count == 0 || Defenders.Count == 0)
            {
                Trace.WriteLine(Name + "Annihilation : War ended on " + StartDate.AddDays(DaysElapsed));
                res = true;
            }
            else
            {
                computeCasualties();
                //We compute the probability of a battle, depending on the number of countries involved in the war :
                if (Random.NextDouble() < 0.05)
                {
                    computeBattle();
                }



                
            }
            
            


            if (timeToLive == 0)
            {
                if (Random.NextDouble() < probabilityOfEndingConflict)
                {
                    Trace.WriteLine(Name+"Draw : War ended on " + StartDate.AddDays(DaysElapsed));
                    res = true;
                }
                else
                {
                    probabilityOfEndingConflict *= 2;
                    timeToLive = (scoreDef + scoreAtt) * Random.Next(100);
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
                attackersPoint += (int)n.Military;
            }

            foreach (Nation n in Defenders)
            {
                defendersPoint += (int)n.Military;
            }

            totalMilitaryPoints = attackersPoint - defendersPoint +  Random.Next(-defendersPoint, attackersPoint);

            if (totalMilitaryPoints > 0) // Attacker wins the day !
            {
                if (attacker.CurrentVictoryPoints < attacker.VictoryPoints)
                {
                    attacker.CurrentVictoryPoints++;
                }
                else
                {
                    defender.CurrentVictoryPoints--;
                    if (defender.CurrentVictoryPoints == 0)
                    {
                        Trace.WriteLine(this.Name + defender.Code + " has been defeated !");
                        RemoveDefender(defender);
                    }
                }
                //Trace.WriteLine(this.Name + attacker.Code + " wins the day !");
                attacker.Military -= Random.Next(0, (int)defender.Military);
                defender.Military -= Random.Next(0, (int)attacker.Military);

            }
            else //Defender wins the day !
            {
                if (defender.CurrentVictoryPoints < defender.VictoryPoints)
                {
                    defender.CurrentVictoryPoints++;
                }
                else
                {
                    attacker.CurrentVictoryPoints--;
                    if (attacker.CurrentVictoryPoints == 0)
                    {
                        Trace.WriteLine(this.Name + attacker.Code + " has been defeated !");
                        RemoveAttacker(attacker);

                    }
                }
                //Trace.WriteLine(this.Name+defender.Code + " wins the day !");
                decimal tmp = attacker.Military /1000;
                attacker.Military -= Random.Next(0, defender.Military/1000);
                defender.Military -= Random.Next(0, tmp);

            }

            
        }

        private void computeCasualties()
        { 
            Nation attacker = Attackers[Random.Next(0, Attackers.Count)];
            foreach (Nation n in Defenders)
            {
                n.Population -= Random.Next(0,
                    (attacker.Military * n.Population) / 1000000);

            }
            Nation defender = Defenders[Random.Next(0, Defenders.Count)];
            foreach (Nation n in Attackers)
            {
                n.Population -= Random.Next(0, (defender.Military * n.Population) / 1000000)  ; 
            }
            
          
        }


        public War(List<Nation> attackers, List<Nation> defenders, DateTime startDate)
        {
            Name = attackers.First().Code + " VS " + defenders.First().Code + ":";
            Attackers = attackers;
            Defenders = defenders;
            StartDate = startDate;
            DaysElapsed = 0;
            numberOfCountries = attackers.Count + defenders.Count;
            CodeAttackers = new List<Ecode>();
            CodeDefenders = new List<Ecode>();
            foreach (Nation n in attackers)
            {
                CodeAttackers.Add(n.Code);
            }
            foreach (Nation n in defenders)
            {
                CodeDefenders.Add(n.Code);
            }

            scoreDef = 0;
            scoreAtt = 0;
            probabilityOfEndingConflict = 0.1;

            foreach(Nation n in defenders)
            {
                scoreDef += n.CurrentVictoryPoints;
                n.NbWarEngagedIn ++;
            }


            foreach (Nation n in attackers)
            {
                scoreAtt += n.CurrentVictoryPoints;
                n.NbWarEngagedIn++;
            }
            
            timeToLive =(scoreDef+ scoreAtt) * Random.Next(100);

            Trace.WriteLine("New War on " + StartDate + " between " + attackers.First().Code + "(attacker) and " + defenders.First().Code +" (defender)");
            Trace.WriteLine("Time to live : " + timeToLive);
            Trace.WriteLine("Score of defender : " + scoreDef);
            Trace.WriteLine("Score of attacker : " + scoreAtt);
        }

        public void AddAttacker(Nation n)
        {
            Attackers.Add(n);
            CodeAttackers.Add(n.Code);
            scoreAtt += n.CurrentVictoryPoints;
            n.NbWarEngagedIn ++;
            Trace.WriteLine(Name+n.Code+" has joined the war on the side of the attackers !");
            numberOfCountries++;
        }

        public void AddDefender(Nation n)
        {
            Defenders.Add(n);
            CodeDefenders.Add(n.Code);
            n.NbWarEngagedIn++;
            scoreDef += n.CurrentVictoryPoints;
            Trace.WriteLine(Name+n.Code+" has joined the war on the side of the defenders !");
            numberOfCountries++;
        }

        public void RemoveAttacker(Nation n)
        {
            Nation strongestDefender = Defenders.OrderByDescending(n => n.Military).FirstOrDefault();

           


            AnnexationOccurred?.Invoke(this, new AnnexionNationEventArgs(n, false));

           DisengageAttacker(n);
        }

        public void RemoveDefender(Nation n)
        {
            

          

            //Call annex function with n and this as args
            AnnexationOccurred?.Invoke(this, new AnnexionNationEventArgs(n,true));

            DisengageDefender(n);
        }

        public void DisengageDefender(Nation n)
        {
            Defenders.Remove(n);
            CodeDefenders.Remove(n.Code);
            scoreDef -= n.CurrentVictoryPoints;
            numberOfCountries--;
            n.NbWarEngagedIn--;
        }

        public void DisengageAttacker(Nation n)
        {
            Attackers.Remove(n);
            CodeAttackers.Remove(n.Code);
            scoreAtt -= n.CurrentVictoryPoints;
            numberOfCountries--;
            n.NbWarEngagedIn--;
        }

    }
}
