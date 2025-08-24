namespace YinStudio.Models
{
    public class Onsite : Class
    {
        public int RoomNumber { get; set; }
        public ICollection<Equipment> EquipmentAvailable { get; set; } = new List<Equipment>();
    }
}