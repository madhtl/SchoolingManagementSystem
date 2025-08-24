using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace YinStudio.Models;

public class Timeslot : IValidatableObject{
    [Key]
    public int IdTimeslot { get; set; }
    [Required]
    public DateTime Date { get; set; }
    [Required]
    public TimeSpan HourFrom { get; set; }
    [Required]
    public TimeSpan HourTo { get; set; }
    
    public int? IdTimetable { get; set; }
    [JsonIgnore]
    public Timetable? Timetable { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (HourFrom >= HourTo)
        { yield return new ValidationResult(
                "HourFrom must be earlier than HourTo.",
                new[] { nameof(HourFrom), nameof(HourTo) });
        }
    }
}