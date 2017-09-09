using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSQLScreen.Models
{
    public class LoginHistory
    {
        public int Id { get; set; }

        public DateTime? LoginDate { get; set; }

        public AdminAccount AdminAccount { get; set; }

        public int? AdminAccountId { get; set; }
    }
}