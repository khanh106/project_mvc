using Microsoft.AspNetCore.Identity;

namespace tao_project.Models

{
    public class ApplicationUser:IdentityUser
    {
        [PersonalData]
        public string FullName { get; set; }
    }
}