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


    //General plans:
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

    public class Stakhanovism : PoliticalPlan
    {
        public Stakhanovism() : base(" Stakhanovisme (Productivité + 1%, Contentement - 1%) \r\n\"Let’s show to the entire world the spirit of Chollima!\r\nLet's go quicker, let's go faster, riding on the Chollima\r\nForward, let’s hasten the 7-year-plan!\" ") { }
        protected internal override void init() { this.maxcpt = 50; this.cpt = 0; }
        protected internal override void takeEffect(Nation n) { n.Productivity += 0.01;  n.HappinessRate -= 0.01; }
    }

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




}
