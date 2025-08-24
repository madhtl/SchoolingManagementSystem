using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace YinStudio.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Onsite), "Onsite")]
[JsonDerivedType(typeof(Online), "Online")]
public class Class
{
    [Key]
    public int IdClass { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    public DifficultyLevel DifficultyLevel { get; set; }

    [MaxLength(150)]
    public string? Description { get; set; }

    public decimal Price { get; set; }

    public ClassStatus Status { get; set; }
    [JsonIgnore]
    
    public int? InstructorId { get; set; }
    [JsonIgnore]
    public Instructor? Instructor { get; set; }

    [JsonIgnore]
    public ICollection<Timeslot> Timeslots { get; set; } = new HashSet<Timeslot>();

    [JsonIgnore]
    public ICollection<Customer> Customers { get; set; } = new HashSet<Customer>();

    [JsonIgnore]
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}


    public enum ClassStatus
    {
        Unassigned,
        Assigned
    }
    public enum DifficultyLevel
    {
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3
    }
