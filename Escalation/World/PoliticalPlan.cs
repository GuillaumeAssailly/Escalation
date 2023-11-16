using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Animation;

namespace Escalation.World
{
    public abstract class PoliticalPlan
    {
        protected string description;

        protected int cpt;

        protected int maxcpt;

        public PoliticalPlan(string description)
        {
            this.description = description;
        }

        protected internal abstract void init();
        protected internal abstract void takeEffect(Nation n);
        protected internal void dayPass() { this.cpt++; }

        protected internal bool isFinished()
        {
            return this.cpt == maxcpt; }

        protected internal int getMaxCpt() { return this.maxcpt; }

        public double GetProgress()
        {
            return (double)this.cpt / (double)this.maxcpt;
        }

        public string GetDescription()
        {
            return this.description;
        }
    }


    // Income Plan : 
    // Plan that mostly add Income
    public class IndustrialPlan : PoliticalPlan
    {
        public IndustrialPlan() : base(" Extension industrielle (IndustrialPower + 5) :  ce pays augmente son parc industriel") {}
        protected internal override void init() { this.maxcpt = 50; this.cpt = 0; }
        protected internal override void takeEffect(Nation n) { n.IndustrialPower += 5; }
    }

    public class AgriculturalPlan : PoliticalPlan
    {
        public AgriculturalPlan() : base("Plan agricole (AgriculturalPower + 5): ce pays augmente ses capacités agricoles ") { }
        protected internal override void init() { this.maxcpt = 40; this.cpt = 0; }
        protected internal override void takeEffect(Nation n) { n.AgriculturalPower += 5; }
    }

    public class TertiaryPlan : PoliticalPlan
    {
        public TertiaryPlan() : base(" Plan de relance du tertiaire (TertiaryPower + 5) : ce pays augmente ses services tertiaires ") { }
        protected internal override void init() { this.maxcpt = 20; this.cpt = 0; }
        protected internal override void takeEffect(Nation n) { n.TertiaryPower += 5; }
    }

    public class DismantlingIndustrialPlan : PoliticalPlan
    {
        public DismantlingIndustrialPlan() : base(" Démantèlement industriel (IndustrialPower - 5, TertiaryPower + 2)  : ce pays démantèle une partie de son industrie au profit de services du tertiaire") { }
        protected internal override void init() { this.maxcpt = 20; this.cpt = 0; }
        protected internal override void takeEffect(Nation n) { n.IndustrialPower -= 5; n.TertiaryPower += 2; }
    }

    public class DismantlingAgriculturalPlan : PoliticalPlan
    {
        public DismantlingAgriculturalPlan() : base(" Démantèlement agricole (AgriculturalPower - 5, TertiaryPower + 2)  : ce pays démantèle une partie de son agriculture au profit de services du tertiaire") { }
        protected internal override void init() { this.maxcpt = 20; this.cpt = 0; }
        protected internal override void takeEffect(Nation n) { n.AgriculturalPower -= 5; n.TertiaryPower += 2; }
    }

  


    //Expenses Plan : 
    public class PublicHealth : PoliticalPlan
    {
        public PublicHealth() : base(" Plan de santé publique (HealthRate + 5%) : ce pays investit dans l'hôpital et la santé publique.") { }
        protected internal override void init() { this.maxcpt = 75; this.cpt = 0; }
        protected internal override void takeEffect(Nation n) { n.HealthRate += 0.05; }
    }

    public class EducationPlan : PoliticalPlan
    {
        public EducationPlan() : base(" Plan d'éducation (EducationRate + 5%) : ce pays investit dans l'éducation et la formation de sa population") { }
        protected internal override void init() { this.maxcpt = 50; this.cpt = 0; }
        protected internal override void takeEffect(Nation n) { n.EducationRate += 0.05; }
    }

    public class FoodRatePlan : PoliticalPlan
    {
        public FoodRatePlan() : base("Politique de satiété (Foodrate + 5%) : \n\"Ne donnez pas un poisson à un enfant mais montrez-lui comment le pêcher\"\nMao Zedong") { }
        protected internal override void init() { this.maxcpt = 100; this.cpt = 0; }
        protected internal override void takeEffect(Nation n) { n.FoodRate += 0.05; }
    }


    public class NatalityPlan : PoliticalPlan
    {
        public NatalityPlan() : base("Politique de natalité (Natalité + 5%) : ce pays régule ses naissances et l'engage dans une politique de surnatalité.") { }
        protected internal override void init() { this.maxcpt = 200; this.cpt = 0; }
        protected internal override void takeEffect(Nation n) { n.PopulationGrowthRate += 0.05 * n.PopulationGrowthRate; }
    }

    /*
    public class MilitaryPlan : PoliticalPlan
    {
        public MilitaryPlan() : base("Plan militaire (MilitaryPower + 5) : ce pays augmente ses capacités militaires") { }
        protected internal override void init() { this.maxcpt = 50; this.cpt = 0; }
        protected internal override void takeEffect(Nation n) { n. += 5; }
    }

    */

    //Political Ideologies Specific plan : 
    //Communism : 
    public class Stakhanovism : PoliticalPlan
    {
        public Stakhanovism() : base(" Stakhanovisme (Productivité + 1%, Contentement - 1%) \r\n\"Let’s show to the entire world the spirit of Chollima!\r\nLet's go quicker, let's go faster, riding on the Chollima\r\nForward, let’s hasten the 7-year-plan!\" ") { }
        protected internal override void init() { this.maxcpt = 50; this.cpt = 0; }
        protected internal override void takeEffect(Nation n) { n.Productivity += 0.01; n.HappinessRate -= 0.01; }
    }

  
    public class FiveYearPlan : PoliticalPlan
    {
        public FiveYearPlan() : base(" Plan quinquennal (IndustrialPower + 5, AgriculturalPower + 5, TertiaryPower + 5, Productivity + 1%)\" ") { }
        protected internal override void init() { this.maxcpt = 365; this.cpt = 0; }
        protected internal override void takeEffect(Nation n) { n.IndustrialPower += 10; n.AgriculturalPower += 10; n.TertiaryPower += 5; n.Productivity += 0.01; }
    }

    //Socialist :
    public class PublicSocialism : PoliticalPlan
    {
        public PublicSocialism() : base(" Augmentation des services publics (HealthRate + 5%, EducationRate + 5%, FoodRate + 5%, Productivity + 1%) \r\n\"From each according to his ability, to each according to his needs\" ") { }
        protected internal override void init() { this.maxcpt = 200; this.cpt = 0; }
        protected internal override void takeEffect(Nation n) { n.HealthRate += 0.05; n.EducationRate += 0.05; n.FoodRate += 0.05; n.Productivity += 0.01; }
    }

    public class EcologicalPlan : PoliticalPlan
    {
        public EcologicalPlan() : base("Démentelement de l'industrie lourde polluante (IndustrialPower - 5%, HealthRate + 5%)\" ") { }
        protected internal override void init() { this.maxcpt = 200; this.cpt = 0; }
        protected internal override void takeEffect(Nation n) { n.HealthRate += 0.05; n.IndustrialPower -= 5; }
    }
    
    //Alt Right Plan :
    public class IndustrialPrivatisationPlan : PoliticalPlan
    {
        public IndustrialPrivatisationPlan() : base(" Plan de privatisation industriel (IndustrialPower - 5, Productivity + 1%)") { }
        protected internal override void init() { this.maxcpt = 100; this.cpt = 0; }
        protected internal override void takeEffect(Nation n) { n.IndustrialPower -= 5; n.Productivity += 0.01; }
    }

    public class AgriculuturalPrivatisation : PoliticalPlan
    {
        public AgriculuturalPrivatisation() : base(" Plan de privatisation agricole (AgriculturalPower - 5, Productivity + 1%)") { }
        protected internal override void init() { this.maxcpt = 100; this.cpt = 0; }
        protected internal override void takeEffect(Nation n) { n.AgriculturalPower -= 5; n.Productivity += 0.01; }
    }

    public class DestroyingPublicService : PoliticalPlan
    {
        public DestroyingPublicService() : base(" Privatisation des services publiques ( HealthRate - 10%, EducationRate - 5%, Productivity + 3%) ") { }
        protected internal override void init() { this.maxcpt = 150; this.cpt = 0; }
        protected internal override void takeEffect(Nation n) { n.HealthRate -= 0.05; n.EducationRate -= 0.05; n.Productivity += 0.03;  }
    }

    //Authoritarianism Plans :



    //Despotism Plans : 
    public class DespoticRule : PoliticalPlan
    {
        public DespoticRule() : base("Gouvernance despotique (Productivité + 5%, Contentement - 2 %, FoodRate - 3%) ") { }
        protected internal override void init() { this.maxcpt = 25; this.cpt = 0; }
        protected internal override void takeEffect(Nation n) { n.Productivity += 0.05; n.HappinessRate -= 0.02; n.FoodRate -= 0.03; }
    }



    //Fascists plans  : 
    public class DestroyingSocialRights : PoliticalPlan
    {
        public DestroyingSocialRights() : base(" Régression d'acquis sociaux (Productivité + 2%, Contentement - 10 %) ") { }
        protected internal override void init() { this.maxcpt = 200; this.cpt = 0; }
        protected internal override void takeEffect(Nation n) { n.Productivity += 0.02; n.HappinessRate -= 0.1; }
    }

    public class TotalitarianRepression : PoliticalPlan
    {
        public TotalitarianRepression() : base("Répression totalitaire (Stabilité + 1%, Population - 5%) ") { }
        protected internal override void init() { this.maxcpt = 20; this.cpt = 0; }
        protected internal override void takeEffect(Nation n) { n.Stability += 0.05; n.Population -= (decimal)0.05 * n.Population;
            Console.WriteLine(n.Population);
        }

    }
}
