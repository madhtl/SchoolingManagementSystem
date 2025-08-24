using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace YinStudio.Models;
public class Customer : Person
{
    [Required]
    public DifficultyLevel ClassLevel { get; set; }

    public string? SpecialNeeds { get; set; }
    [JsonIgnore]
    public ICollection<Exp> Exps { get; set; } = new HashSet<Exp>();
    [JsonIgnore]
    public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    [JsonIgnore]
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    [JsonIgnore]
    public ICollection<Class> Classes { get; set; } = new List<Class>();

}
