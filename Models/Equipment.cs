using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace YinStudio.Models;

public class Equipment
{
    [Key]
    public int IdEquipment { get; set; }

    public string Name { get; set; } = null!;
    
    [JsonIgnore]
    public ICollection<Onsite> Onsite { get; set; } = new List<Onsite>();  // must match WithMany(e => e.Onsite)
}

