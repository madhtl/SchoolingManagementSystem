using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace YinStudio.Models;

public class Instructor : Person
{
    [Required]
    public string ClassType { get; set; }
    [JsonIgnore]

    public bool Available { get; set; }

    [Required]
    [StringLength(100)]
    public string LicenseNo { get; set; }
    
    [JsonIgnore]
    public Timetable Timetable { get; set; }
}
