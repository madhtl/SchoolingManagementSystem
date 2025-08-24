using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using YinStudio;
using YinStudio.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    // polymorphic JSON 
});

builder.Services.AddRazorPages();

// Register the database context
builder.Services.AddDbContext<YinStudioContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

/*
// Uncomment if CORS is needed
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
*/

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<YinStudioContext>();
    var initializer = new DbInitializer(dbContext);

    string jsonFilePath = "/Users/madzia/RiderProjects/YinStudio/data.json";
    await initializer.SeedAsync(jsonFilePath);
}

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Optional for development
// app.UseCors();             // Enable if needed

app.UseAuthorization();

app.MapControllers();

app.MapRazorPages();

await app.RunAsync();