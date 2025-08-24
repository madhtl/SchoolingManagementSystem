namespace YinStudio;
using YinStudio.Models;

public class RootData
{
    public List<Person> People { get; set; }
    public List<Membership> Memberships { get; set; }
    public List<Exp> Exps { get; set; }
    public List<Order> Orders { get; set; }
    public List<Review> Reviews { get; set; }
    public List<Timeslot> Timeslots { get; set; }
    public List<Timetable> Timetables { get; set; }
    public List<Class> Classes { get; set; }
    public List<Equipment> Equipment { get; set; }
}