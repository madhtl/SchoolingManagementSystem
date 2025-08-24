using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace YinStudio.Models;

public class Timetable
{
    [Key]
    public int IdTimetable { get; set; }
    [Required]
    public string Version { get; set; }
    [Required]
    public int InstructorId { get; set; }

    [JsonIgnore]
    public Instructor? Instructor { get; set; }
    [JsonIgnore]
    public ICollection<Timeslot> Timeslots { get; set; } = new HashSet<Timeslot>();
}