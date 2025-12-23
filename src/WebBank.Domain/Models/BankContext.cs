using Microsoft.EntityFrameworkCore;

namespace WebBank.Domain.Models
{
    public class BankContext : DbContext
    {
        // Costruttore obbligatorio per la configurazione
        public BankContext(DbContextOptions<BankContext> options) : base(options)
        {
        }

        // Questa riga crea la tabella "Accounts" nel database
        public DbSet<CurrentAccount> Accounts { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Spieghiamo a EF che CurrentAccountPlus eredita da CurrentAccount
            modelBuilder.Entity<CurrentAccountPlus>().HasBaseType<CurrentAccount>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
