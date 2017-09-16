using System.ComponentModel.DataAnnotations;

namespace MSSQLScreen.Models
{
    public class AddAdminViewModel
    {
        [Required]
        public string Username { get; set; }
    }
}