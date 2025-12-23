namespace WebBank.Domain.Models
{
    public enum AccountStatus
    {
        Empty,
        Positive,
        Negative,
    }

    public class CurrentAccount
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        // CORREZIONE FONDAMENTALE PER IL WEB:
        // Public get: JavaScript può leggerlo.
        // Protected set: Solo le classi figlie (Plus) possono toccarlo.
        public double Balance { get; protected set; }

        public AccountStatus CurrentStatus
        {
            get
            {
                if (Balance == 0) return AccountStatus.Empty;
                return Balance > 0 ? AccountStatus.Positive : AccountStatus.Negative;
            }
        }

        public CurrentAccount(Guid id, string name, double balance)
        {
            Id = id;
            Name = name;
            Balance = balance;
        }

        public virtual bool Deposit(double amount)
        {
            if (amount < 0) return false;
            Balance += amount;
            return true;
        }

        public virtual bool Withdrawal(double amount)
        {
            // Qui controlliamo solo se l'importo è positivo, la logica del segno la gestisce chi chiama
            if (amount < 0) return false;
            if (amount > Balance) return false;

            Balance -= amount;
            return true;
        }

        public void AddInterests()
        {
            if (Balance >= 100 && Balance <= 1000)
                Balance += Balance * 0.02;
            else if (Balance > 1000)
                Balance += Balance * 0.04;
        }

        // Questo metodo restituisce un oggetto anonimo o una stringa formattata, utile per debug
        public virtual string GetInformation()
        {
            return $"{Name} | {Id} | {CurrentStatus} | {Balance}";
        }
    }
}
