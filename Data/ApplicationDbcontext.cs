using  Microsoft.EntityFrameworkCore;
using tao_project.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using tao_project.Models.Entities;

namespace tao_project.Data{

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }


    public DbSet<Person> Persons { get; set; }
    public DbSet<MemberUnit> MemberUnits { get; set; }
    public DbSet<Employee> Employees { get; set; }
    

}


}