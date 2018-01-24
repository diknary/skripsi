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

    }
}