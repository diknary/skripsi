using System.ComponentModel.DataAnnotations;

namespace MSSQLScreen.Models
{
    public class UserAccount
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string NIP { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Privilege { get; set; }
    }
}