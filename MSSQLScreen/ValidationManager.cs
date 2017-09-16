using System.Linq;
using MSSQLScreen.Models;

namespace MSSQLScreen
{
    public class ValidationManager
    {
        private ApplicationDbContext _context;

        public ValidationManager()
        {
            _context = new ApplicationDbContext();
        }

        public bool IsValid(string username, string password)
        {
            return _context.AdminAccounts.Any(c => c.Username == username && c.Password == password);
        }
    }
}