using System.ComponentModel.DataAnnotations;

namespace MSSQLScreen.Models
{
    public class ServerList
    {
        public int Id { get; set; }

        [Required]
        public string IPAddress { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Salt { get; set; }

        public bool IsActive { get; set; }
    }
}