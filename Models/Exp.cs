namespace YinStudio.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Exp
{
    [Key]
    public int IdExp { get; set; }
    
    public string? Description { get; set; }

    [Required]
    public DateTime ExpirationDate { get; set; }
    
    [Required]
    public int IdCustomer { get; set; }

    [JsonIgnore]
    public Customer? Customer { get; set; }

    [Required]
    public int IdMembership { get; set; }

    [JsonIgnore]
    public Membership? Membership { get; set; }
}