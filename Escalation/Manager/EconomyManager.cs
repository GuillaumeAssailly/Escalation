using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Escalation.World;
using Random = Escalation.Utils.Random;

namespace Escalation.Manager
{
    public class EconomyManager : Manager

    {
        public EconomyManager(Earth World, Random Random) : base(World, Random)
        {
        }

       
        public void ManageEconomy(Ecode code)
        {
            Nation CurrentNation = World.Nations[(int)code];

           

            if (CurrentNation != null)
            {
                CurrentNation.UpdateTreasury();
            }

            //Remove the first element of the list :
            if (CurrentNation.TreasuryHistory.Count > 12)
            {
                CurrentNation.TreasuryHistory.RemoveAt(0);
                CurrentNation.ExpensesHistory.RemoveAt(0);
                CurrentNation.IncomesHistory.RemoveAt(0);
                CurrentNation.DebtHistory.RemoveAt(0);

            }
            CurrentNation.TreasuryHistory.Add(CurrentNation.Treasury);
            CurrentNation.ExpensesHistory.Add(CurrentNation.Expenses);
            CurrentNation.IncomesHistory.Add(CurrentNation.Incomes);
            CurrentNation.DebtHistory.Add(CurrentNation.Debt);


        }


    }
}
