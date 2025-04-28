using System.ComponentModel.DataAnnotations;
namespace tao_project.Models;

public class Employee : Person
{
    public string EmployeeID { get; set; }
    public int? Age { get; set; }
}