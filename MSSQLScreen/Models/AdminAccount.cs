using System;
using System.ComponentModel.DataAnnotations;

namespace MSSQLScreen.Models
{
    public class AdminAccount
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string NIP { get; set; }

        public string Username { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Privilege { get; set; }

        public DateTime? LastLogin { get; set; }

        public bool IsConnected { get; set; }
    }
}