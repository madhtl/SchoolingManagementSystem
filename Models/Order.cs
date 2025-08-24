using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace YinStudio.Models;

public class Order
{
    [Key]
    public int IdOrder { get; set; }
    public DateTime Date { get; set; }
    public string? Wishes { get; set; }
    [Required]
    public int CustomerId { get; set; }
    [JsonIgnore]
    public Customer? Customer { get; set; }
    public int? ClassId { get; set; }
    [JsonIgnore]
    public Class? Class { get; set; }
    public int? MembershipId { get; set; } 
    [JsonIgnore]
    public Membership? Membership { get; set; }

}