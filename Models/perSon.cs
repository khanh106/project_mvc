using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace tao_project.Models;

[Table("Persons")]
public partial class Person
{
   [Key]
    public string? Id { get; set; }

    public string? Fullname { get; set; }

    public string? Address { get; set; }
}
