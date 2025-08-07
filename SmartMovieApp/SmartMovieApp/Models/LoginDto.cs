using System.ComponentModel.DataAnnotations;

namespace SmartMovieApp.Models
{
    public class LoginDto
    {
        [Required]
        public string UsernameOrEmail { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
