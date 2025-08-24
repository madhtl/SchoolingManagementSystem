using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace YinStudio.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Instructor), "Instructor")]
[JsonDerivedType(typeof(Customer), "Customer")]
[JsonDerivedType(typeof(Admin), "Admin")]
public abstract class Person
{
    [Key]
    public int IdPerson { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    [MaxLength(50)]
    public string Surname { get; set; }

    [Required]
    public string Address { get; set; }
}