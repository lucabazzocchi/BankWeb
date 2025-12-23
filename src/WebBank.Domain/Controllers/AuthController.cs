using Microsoft.AspNetCore.Mvc;
using WebBank.Domain.Models;
using System.Linq;

namespace WebBank.Domain.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly BankContext _context;

        public AuthController(BankContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRegistration req)
        {
            if (_context.Users.Any(u => u.Username == req.Username))
                return BadRequest("Utente già esistente");

            var newUser = new User
            {
                Username = req.Username,
                PasswordHash = req.Password // Per ora semplice, poi metteremo l'hash vero
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();
            return Ok(new { message = "Registrazione completata" });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserRegistration req)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == req.Username && u.PasswordHash == req.Password);

            if (user == null) return Unauthorized("Credenziali errate");

            return Ok(new { id = user.Id, username = user.Username });
        }
    }

    public class UserRegistration { public string Username { get; set; } public string Password { get; set; } }
}