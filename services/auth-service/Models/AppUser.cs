using Microsoft.AspNetCore.Identity;

namespace auth_service.Models
{
    public class AppUser : IdentityUser
    {
        public string? Name { get; set; }
    }
}