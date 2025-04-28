using Microsoft.AspNetCore.Identity;

namespace tao_project.Models

{
    public class ApplicationUser:IdentityUser
    {
        public string? Fullname { get; set; }
    }
}