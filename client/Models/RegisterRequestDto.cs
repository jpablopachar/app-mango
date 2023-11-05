using System.ComponentModel.DataAnnotations;

namespace client.Models
{
    public class RegisterRequestDto
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Phone { get; set; }
        [Required]
        public string? Password { get; set; }
        public string? Role { get; set; }
    }
}