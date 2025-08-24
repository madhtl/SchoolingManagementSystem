using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YinStudio.Models;

public class Admin : Person
{
    public List<string> Permissions { get; set; } = new();
}
