using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YinStudio;
using YinStudio.Models;


public class DbInitializer
{
    private readonly YinStudioContext _context;

    public DbInitializer(YinStudioContext context)
    {
        _context = context;
    }

public async Task SeedAsync(string jsonFilePath)
{
    string jsonString = await File.ReadAllTextAsync(jsonFilePath);
    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    RootData data = JsonSerializer.Deserialize<RootData>(jsonString, options);

    // 1. Clear tables in reverse dependency order
    await _context.Database.ExecuteSqlRawAsync("DELETE FROM CustomerClass"); // junction table
    await _context.Database.ExecuteSqlRawAsync("DELETE FROM Orders");
    await _context.Database.ExecuteSqlRawAsync("DELETE FROM Reviews");
    await _context.Database.ExecuteSqlRawAsync("DELETE FROM Exps");
    await _context.Database.ExecuteSqlRawAsync("DELETE FROM Equipment");
    await _context.Database.ExecuteSqlRawAsync("DELETE FROM Timeslots");
    await _context.Database.ExecuteSqlRawAsync("DELETE FROM Timetables");
    await _context.Database.ExecuteSqlRawAsync("DELETE FROM Classes");
    await _context.Database.ExecuteSqlRawAsync("DELETE FROM Memberships");
    await _context.Database.ExecuteSqlRawAsync("DELETE FROM People");

    // 2. Reseed identity columns (if using SQL Server and identity columns)
    await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('People', RESEED, 0);");
    await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Memberships', RESEED, 0);");
    await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Exps', RESEED, 0);");
    await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Orders', RESEED, 0);");
    await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Reviews', RESEED, 0);");
    await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Timeslots', RESEED, 0);");
    await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Timetables', RESEED, 0);");
    await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Classes', RESEED, 0);");
    await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Equipment', RESEED, 0);");
    

    if (data.Memberships?.Any() == true)
        await _context.Memberships.AddRangeAsync(data.Memberships);

    if (data.People?.Any() == true)
        await _context.People.AddRangeAsync(data.People);

    if (data.Classes?.Any() == true)
        await _context.Classes.AddRangeAsync(data.Classes);

    await _context.SaveChangesAsync();

    // 4. Add Timetables before Timeslots
    if (data.Exps?.Any() == true)
    {
        await _context.Exps.AddRangeAsync(data.Exps);
        await _context.SaveChangesAsync();
    }
    if (data.Timetables?.Any() == true)
    {
        await _context.Timetables.AddRangeAsync(data.Timetables);
        await _context.SaveChangesAsync(); // Important: save so Timetable IDs exist
    }

    // 5. Add Timeslots referencing Timetables
    if (data.Timeslots?.Any() == true)
    {
        await _context.Timeslots.AddRangeAsync(data.Timeslots);
        await _context.SaveChangesAsync();
    }

    // 6. Add Orders after People and Classes exist
    if (data.Orders?.Any() == true)
    {
        await _context.Orders.AddRangeAsync(data.Orders);
        await _context.SaveChangesAsync();
    }
    
    if (data.Reviews?.Any() == true)
    {
        await _context.Reviews.AddRangeAsync(data.Reviews);
        await _context.SaveChangesAsync();
    }

    if (data.Equipment?.Any() == true)
        await _context.Equipment.AddRangeAsync(data.Equipment);

    await _context.SaveChangesAsync();
}
}
