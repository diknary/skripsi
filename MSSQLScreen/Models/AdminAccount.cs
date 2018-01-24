using System;
using System.ComponentModel.DataAnnotations;

namespace MSSQLScreen.Models
{
    public class AdminAccount
    {
        public int Id { get; set; }

        public UserAccount UserAccount { get; set; }

        public int? UserAccountId { get; set; }

        public string Privilege { get; set; }

        public DateTime? LastLogin { get; set; }

        public byte IsOnline { get; set; }

        public string FirebaseToken { get; set; }
    }
}