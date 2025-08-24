using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace YinStudio.Models;

public class Review
{
    [Key]
    public int IdReview { get; set; }

    [Range(1, 5)]
    public int Ranking { get; set; }

    public string? Text { get; set; }
    [Required]
    public int IdCustomer { get; set; }
    [JsonIgnore]
    public Customer? Customer { get; set; } = null!;
    [Required]
    public int IdClass { get; set; }
    [JsonIgnore]
    public Class? Class { get; set; } = null!;

}