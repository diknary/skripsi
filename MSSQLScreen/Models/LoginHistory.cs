using System;

namespace MSSQLScreen.Models
{
    public class LoginHistory
    {
        public int Id { get; set; }

        public DateTime? LoginDate { get; set; }

        public AdminAccount AdminAccount { get; set; }

        public int? AdminAccountID { get; set; }
    }
}