namespace YinStudio;

using Microsoft.EntityFrameworkCore;
using YinStudio.Models;

public partial class YinStudioContext : DbContext
{
    public YinStudioContext() { }

    public YinStudioContext(DbContextOptions<YinStudioContext> options) : base(options) { }

    public DbSet<Person> People { get; set; }
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<Exp> Exps { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Timeslot> Timeslots { get; set; }
    public DbSet<Timetable> Timetables { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<Equipment> Equipment { get; set; }


    


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>()
            .HasDiscriminator<string>("PersonType")
            .HasValue<Instructor>("Instructor")
            .HasValue<Customer>("Customer")
            .HasValue<Admin>("Admin");
        
        modelBuilder.Entity<Exp>()
            .HasOne(e => e.Customer)
            .WithMany(c => c.Exps)
            .HasForeignKey(e => e.IdCustomer)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Exp>()
            .HasOne(e => e.Membership)
            .WithMany(m => m.Exps)
            .HasForeignKey(e => e.IdMembership)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Membership>()
            .Property(m => m.Price)
            .HasColumnType("decimal(10,2)");
// Order belongs to one Customer; deleting Customer deletes Orders
        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Orders)
            .WithOne(o => o.Customer)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Timeslot>()
            .HasOne(t => t.Timetable)
            .WithMany(tt => tt.Timeslots)
            .HasForeignKey(t => t.IdTimetable)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Timetable>()
            .HasOne(t => t.Instructor)
            .WithOne(i => i.Timetable)
            .HasForeignKey<Timetable>(t => t.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Class>()
            .Property(c => c.Price)
            .HasColumnType("decimal(10,2)");
        
        modelBuilder.Entity<Instructor>()
            .HasIndex(i => i.LicenseNo)
            .IsUnique();
        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Classes)
            .WithMany(cl => cl.Customers)
            .UsingEntity(j => j.ToTable("CustomerClass")); // optional: name the join table
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Customer)
            .WithMany(c => c.Reviews)
            .HasForeignKey(r => r.IdCustomer)
            .OnDelete(DeleteBehavior.Cascade); // gdy usuwasz klienta

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Class)
            .WithMany(cl => cl.Reviews)
            .HasForeignKey(r => r.IdClass)
            .OnDelete(DeleteBehavior.Cascade); // gdy usuwasz klasę
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasOne(o => o.Membership)
                .WithMany()   // Membership can be linked to many Orders
                .HasForeignKey(o => o.MembershipId)
                .OnDelete(DeleteBehavior.SetNull); // optional: prevent cascade delete if Membership removed

            entity.HasOne(o => o.Class)
                .WithMany()   // Class can be linked to many Orders
                .HasForeignKey(o => o.ClassId)
                .OnDelete(DeleteBehavior.SetNull); // optional

            // Optionally enforce the XOR logic at the database level via a check constraint (if your DB supports it):
            // Example for SQL Server:
            entity.HasCheckConstraint("CK_Order_MembershipId_XOR_ClassId",
                "((MembershipId IS NOT NULL AND ClassId IS NULL) OR (MembershipId IS NULL AND ClassId IS NOT NULL))");
        });
        
        modelBuilder.Entity<Class>()
            .HasDiscriminator<string>("ClassType")
            .HasValue<Class>("Class")
            .HasValue<Onsite>("Onsite")
            .HasValue<Online>("Online");
        modelBuilder.Entity<Onsite>()
            .HasMany(o => o.EquipmentAvailable)
            .WithMany(e => e.Onsite)
            .UsingEntity<Dictionary<string, object>>(
                "OnsiteEquipment",
                j => j.HasOne<Equipment>()
                    .WithMany()
                    .HasForeignKey("EquipmentId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Onsite>()
                    .WithMany()
                    .HasForeignKey("OnsiteId")
                    .OnDelete(DeleteBehavior.Cascade)
            );


        

        base.OnModelCreating(modelBuilder);
    }
}