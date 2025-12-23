using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace WebBank.Domain.Models
{
    public class Bank
    {
        public List<CurrentAccount> Accounts { get; set; }
        private const double standardBalance = 0;

        public Bank()
        {
            Accounts = new List<CurrentAccount>();
        }

        public void CreateAccountPlus(Guid id, string name)
        {
            Accounts.Add(new CurrentAccountPlus(id, name, standardBalance));
        }

        // --- LA TUA LOGICA ORIGINALE ---

        public string GetBiggerHolder()
        {
            // Protezione: se non ci sono conti, ritorna subito per evitare errori
            if (Accounts.Count == 0) return "Nessun conto presente";

            List<string> Holders = new List<string>();
            List<int> Counters = new List<int>();
            string biggerHolder = string.Empty;

            for (int i = 0; i < Accounts.Count; i++)
            {
                if (Holders.Contains(Accounts[i].Name))
                {
                    int index = Holders.IndexOf(Accounts[i].Name);
                    Counters[index] += 1;
                }
                else
                {
                    Holders.Add(Accounts[i].Name);
                    Counters.Add(1);
                }
            }

            // Qui serve System.Linq per usare .Max()
            int maxIndex = Counters.IndexOf(Counters.Max());
            return Holders[maxIndex];
        }

        public string FindMaxPlus()
        {
            List<CurrentAccountPlus> pluses = new List<CurrentAccountPlus>();
            int maxValue = 0;
            string maxPlusOperations = "";

            foreach (var a in Accounts)
            {
                if (a is CurrentAccountPlus)
                {
                    pluses.Add((CurrentAccountPlus)a);
                }
            }

            foreach (var c in pluses)
            {
                if (c.Operations.Count > maxValue)
                {
                    maxValue = c.Operations.Count;
                    maxPlusOperations = c.Name;
                }
            }
            return maxPlusOperations;
        }
    }
}
