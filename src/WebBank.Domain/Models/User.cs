using System.Collections.Generic;
namespace WebBank.Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; } // Mai salvare password in chiaro!

        // Un utente può avere più conti
        public List<CurrentAccount> Accounts { get; set; } = new List<CurrentAccount>();
    }
}
