using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // <--- Importante per l'autoincrement

namespace WebBank.Domain.Models
{
    public class AccountOperation
    {
        [Key] // Dice che questo è l'ID univoco
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // <--- FONDAMENTALE: Dice al DB di fare 1, 2, 3...
        public int Id { get; set; }

        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public string OperationType { get; set; }

        // Costruttore vuoto per Entity Framework (obbligatorio)
        public AccountOperation() { }

        public AccountOperation(double amount, DateTime date)
        {
            Amount = amount;
            Date = date;

            if (Amount > 0)
                OperationType = "Deposit";
            else if (Amount < 0)
                OperationType = "Withdrawal";
            // Rimosso il throw exception per evitare crash se arriva 0, gestiamolo meglio nel controller
        }

        public string Description => $"{Amount} - {Date} - {OperationType}";
    }
}