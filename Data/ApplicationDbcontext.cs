using  Microsoft.EntityFrameworkCore;
using tao_project.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace tao_project.Data{

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }


    public DbSet<Person> Persons { get; set; }

}


}