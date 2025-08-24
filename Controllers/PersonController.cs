using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YinStudio.Models;

namespace YinStudio.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PeopleController : ControllerBase
{
    private readonly YinStudioContext _yinStudioContext;

    public PeopleController(YinStudioContext yinStudioContext)
    {
        _yinStudioContext = yinStudioContext;
    }

    // GET all people
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Person>>> GetPeople()
    {
        return await _yinStudioContext.People.ToListAsync();
    }

    // GET person by id
    [HttpGet("{id}")]
    public async Task<ActionResult<Person>> GetPerson(int id)
    {
        var person = await _yinStudioContext.People.FindAsync(id);
        if (person == null)
            return NotFound();
        return person;
    }

    // POST instructor
    [HttpPost("instructor")]
    public async Task<ActionResult<Instructor>> CreateInstructor(Instructor instructor)
    {
        _yinStudioContext.People.Add(instructor);
        await _yinStudioContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPerson), new { id = instructor.IdPerson }, instructor);
    }

    // POST customer
    [HttpPost("customer")]
    public async Task<ActionResult<Customer>> CreateCustomer(Customer customer)
    {
        _yinStudioContext.People.Add(customer);
        await _yinStudioContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPerson), new { id = customer.IdPerson }, customer);
    }

    // POST admin
    [HttpPost("admin")]
    public async Task<ActionResult<Admin>> CreateAdmin(Admin admin)
    {
        _yinStudioContext.People.Add(admin);
        await _yinStudioContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPerson), new { id = admin.IdPerson }, admin);
    }

    // PUT instructor
    [HttpPut("instructor/{id}")]
    public async Task<IActionResult> UpdateInstructor(int id, Instructor updated)
    {
        if (id != updated.IdPerson)
            return BadRequest("ID mismatch");

        var existing = await _yinStudioContext.People.FindAsync(id);
        if (existing is not Instructor)
            return NotFound();

        _yinStudioContext.Entry(existing).CurrentValues.SetValues(updated);
        await _yinStudioContext.SaveChangesAsync();
        return NoContent();
    }

    // PUT customer
    [HttpPut("customer/{id}")]
    public async Task<IActionResult> UpdateCustomer(int id, Customer updated)
    {
        if (id != updated.IdPerson)
            return BadRequest("ID mismatch");

        var existing = await _yinStudioContext.People.FindAsync(id);
        if (existing is not Customer)
            return NotFound();

        _yinStudioContext.Entry(existing).CurrentValues.SetValues(updated);
        await _yinStudioContext.SaveChangesAsync();
        return NoContent();
    }

    // PUT admin
    [HttpPut("admin/{id}")]
    public async Task<IActionResult> UpdateAdmin(int id, Admin updated)
    {
        if (id != updated.IdPerson)
            return BadRequest("ID mismatch");

        var existing = await _yinStudioContext.People.FindAsync(id);
        if (existing is not Admin)
            return NotFound();

        _yinStudioContext.Entry(existing).CurrentValues.SetValues(updated);
        await _yinStudioContext.SaveChangesAsync();
        return NoContent();
    }

    // DELETE
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePerson(int id)
    {
        var person = await _yinStudioContext.People.FindAsync(id);
        if (person == null)
            return NotFound();

        _yinStudioContext.People.Remove(person);
        await _yinStudioContext.SaveChangesAsync();
        return NoContent();
    }
    [HttpGet("instructor/license/{licenseNo}/timetable")]
    public async Task<ActionResult<Timetable>> GetInstructorTimetableByLicense(string licenseNo)
    {
        var instructor = await _yinStudioContext.People
            .OfType<Instructor>()
            .Include(i => i.Timetable)
            .FirstOrDefaultAsync(i => i.LicenseNo == licenseNo);

        if (instructor == null)
            return NotFound("Instructor not found");

        if (instructor.Timetable == null)
            return NotFound("Timetable not assigned");

        return instructor.Timetable;
    }


    
}
