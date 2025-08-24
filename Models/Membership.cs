namespace YinStudio.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
public enum MembershipType
{
    Basic = 0,
    Premium = 1,
    VIP = 2
}

public class Membership
{
    [Key]
    public int IdMembership { get; set; }

    public MembershipType Type { get; set; }

    [Required]
    public decimal Price { get; set; }
    [JsonIgnore]
    public ICollection<Exp> Exps { get; set; } = new HashSet<Exp>();
    [JsonIgnore]
    public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    [JsonIgnore]
    public bool IsActive => Exps.Any(exp => exp.ExpirationDate.Date >= DateTime.Today);
}