using System.ComponentModel.DataAnnotations;

namespace tao_project.Models;

public class Movie{
    public int ID { get; set; }
    public string? Title { get; set; }
    [DataType(DataType.Date)]
    public DateTime ReleaseDate { get; set; }
    public string? Genre { get; set; }
    public decimal Price { get; set; }
    
}