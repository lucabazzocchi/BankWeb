using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebBank.Domain.Models;

namespace WebBank.Domain.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankController : ControllerBase
    {
        // 1. DICHIARAZIONE DEL DATABASE
        private readonly BankContext _context;

        // 2. IL COSTRUTTORE (Dependency Injection)
        // Questo permette al controller di usare il database creato in Program.cs
        public BankController(BankContext context)
        {
            _context = context;
        }

        // --- FUNZIONI CLASSE BANK (Ora leggono dal DB) ---

        [HttpGet("accounts")]
        public IActionResult GetAll()
        {
            // Ora carichiamo tutti i conti e, se sono di tipo Plus, includiamo le operazioni
            var accounts = _context.Accounts
                                   .Include(a => (a as CurrentAccountPlus).Operations)
                                   .ToList();
            return Ok(accounts);
        }

        [HttpGet("stats/bigger")]
        public IActionResult GetBigger()
        {
            // Usiamo la tua logica originale passando i dati dal DB
            var tempBank = new Bank { Accounts = _context.Accounts.ToList() };
            return Ok(new { result = tempBank.GetBiggerHolder() });
        }

        [HttpGet("stats/max-plus")]
        public IActionResult GetMaxPlus()
        {
            // Carichiamo i Plus con le operazioni per la tua statistica
            var plusAccounts = _context.Accounts
                .OfType<CurrentAccountPlus>()
                .Include(a => a.Operations)
                .Cast<CurrentAccount>()
                .ToList();

            var tempBank = new Bank { Accounts = plusAccounts };
            return Ok(new { result = tempBank.FindMaxPlus() });
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] CreateAccountRequest req)
        {
            // Cerchiamo l'utente nel DB tramite l'ID
            var user = _context.Users.FirstOrDefault(u => u.Id == req.UserId);

            if (user == null) return NotFound("Utente non trovato");

            // Creiamo il nuovo oggetto conto
            var newAccount = new CurrentAccountPlus(Guid.NewGuid(), user.Username, 0);

            // Lo colleghiamo all'utente tramite una proprietà (se l'hai aggiunta) 
            // o semplicemente aggiungendolo al database se c'è una relazione
            _context.Accounts.Add(newAccount);

            // Se hai impostato la relazione nel modello User, usiamo quella:
            user.Accounts.Add(newAccount);

            try
            {
                _context.SaveChanges(); // SALVA NEL DATABASE
                return Ok(new { message = "Conto creato con successo per " + user.Username });
            }
            catch (Exception ex)
            {
                return BadRequest("Errore nel salvataggio: " + ex.Message);
            }
        }

        public class CreateAccountRequest { public int UserId { get; set; } }

        // --- FUNZIONI CLASSE CURRENT ACCOUNT & PLUS ---

        [HttpPost("operation")]
        public IActionResult Operation([FromBody] OpRequest req)
        {
            var acc = FindAccountFromDb(req.Id);
            if (acc == null) return NotFound("Conto non trovato");

            try
            {
                acc.DepositOrWithdrawal(req.Amount);
                _context.SaveChanges(); // SALVA L'OPERAZIONE NEL DB

                return Ok(new
                {
                    message = "Operazione eseguita e salvata",
                    newBalance = acc.Balance,
                    status = acc.CurrentStatus.ToString()
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("interests")]
        public IActionResult AddInterests([FromBody] Guid id)
        {
            var acc = _context.Accounts.FirstOrDefault(a => a.Id == id);
            if (acc == null) return NotFound("Conto non trovato");

            double vecchioSaldo = acc.Balance;
            acc.AddInterests();

            if (acc.Balance != vecchioSaldo)
            {
                _context.SaveChanges(); // SALVA IL NUOVO SALDO DOPO GLI INTERESSI
                return Ok(new { message = "Interessi applicati!", balance = acc.Balance });
            }

            return Ok(new { message = "Nessun interesse applicabile", balance = acc.Balance });
        }

        [HttpGet("{id}/history")]
        public IActionResult GetHistory(Guid id)
        {
            var acc = FindAccountFromDb(id);
            if (acc == null) return NotFound("Conto non trovato o non è Plus");
            return Ok(acc.Operations);
        }

        // Metodo helper aggiornato per cercare nel Database
        // Metodo helper aggiornato per cercare nel Database
        private CurrentAccountPlus FindAccountFromDb(Guid id)
        {
            // Carichiamo il conto. Se è un CurrentAccountPlus, includiamo le operazioni.
            // Questo trucco evita i problemi di "OfType" che abbiamo visto prima.
            var account = _context.Accounts
                .Include(a => (a as CurrentAccountPlus).Operations)
                .FirstOrDefault(a => a.Id == id);

            // Se lo troviamo e siamo sicuri che sia un Plus, lo restituiamo castato
            return account as CurrentAccountPlus;
        }

    }

    public class OpRequest
    {
        public Guid Id { get; set; }
        public double Amount { get; set; }
    }
}