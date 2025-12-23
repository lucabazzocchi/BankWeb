using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBank.Domain.Models
{
    public class CurrentAccountPlus : CurrentAccount
    {
        public List<AccountOperation> Operations { get; set; }

        public CurrentAccountPlus(Guid id, string name, double balance) : base(id, name, balance)
        {
            Operations = new List<AccountOperation>();
        }

        public void DepositOrWithdrawal(double amount)
        {
            if (amount == 0) return; // Se è 0 non facciamo nulla

            if (amount < 0)
            {
                bool success = base.Withdrawal(Math.Abs(amount));
                if (success)
                {
                    // Creiamo l'oggetto SENZA toccare l'ID (lasciamo fare al DB)
                    Operations.Add(new AccountOperation(amount, DateTime.Now));
                }
                else
                {
                    throw new InvalidOperationException("Saldo insufficiente");
                }
            }
            else
            {
                base.Deposit(amount);
                Operations.Add(new AccountOperation(amount, DateTime.Now));
            }
        }

        // Per il Web, è meglio restituire direttamente la lista Operations (che diventerà un array JSON)
        // piuttosto che una stringa concatenata. Ma mantengo il tuo metodo formattato per coerenza.
        public string MovementsList()
        {
            string movements = "";
            foreach (var op in Operations)
            {
                movements += $"{op.Description}\n";
            }
            return movements;
        }
    }
}