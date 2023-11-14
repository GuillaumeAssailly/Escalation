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
        public IndustrialPlan() : base(" Industrial Plan"){}
        protected internal override void init() { this.maxcpt = 50; this.cpt = 0; }
        protected internal override void takeEffect(Nation n) { n.IndustrialPower += 5; }
    }

    public class AgriculturalPlan : PoliticalPlan
    {
        public AgriculturalPlan() : base(" Agricultural Plan") { }
        protected internal override void init() { this.maxcpt = 40; this.cpt = 0; }
        protected internal override void takeEffect(Nation n) { n.AgriculturalPower += 5; }
    }

    public class TertiaryPlan : PoliticalPlan
    {
        public TertiaryPlan() : base(" Tertiary Plan ") { }
        protected internal override void init() { this.maxcpt = 20; this.cpt = 0; }
        protected internal override void takeEffect(Nation n) { n.TertiaryPower += 5; }
    }
}
