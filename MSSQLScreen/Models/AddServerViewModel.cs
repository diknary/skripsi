using System.ComponentModel.DataAnnotations;

namespace MSSQLScreen.Models
{
    public class AddServerViewModel
    {
        [Required]
        public string IPAddress { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}