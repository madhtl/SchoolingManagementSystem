using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YinStudio.Models;

namespace YinStudio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimetableController : ControllerBase
{
    private readonly YinStudioContext _context;

    public TimetableController(YinStudioContext context)
    {
        _context = context;
    }

    // GET: api/timetable
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Timetable>>> GetAll()
    {
        return await _context.Timetables.Include(t => t.Timeslots).ToListAsync();
    }

    // GET: api/timetable/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Timetable>> GetById(int id)
    {
        var timetable = await _context.Timetables
            .Include(t => t.Timeslots)
            .FirstOrDefaultAsync(t => t.IdTimetable == id);

        if (timetable == null) return NotFound();
        return timetable;
    }

    // POST: api/timetable
    // Create timetable, requires InstructorId in timetable object
    [HttpPost]
    public async Task<ActionResult<Timetable>> Create(Timetable timetable)
    {
        // Optionally validate InstructorId exists
        var instructorExists = await _context.People
            .OfType<Instructor>()
            .AnyAsync(i => i.IdPerson == timetable.InstructorId);
        if (!instructorExists)
            return BadRequest("Invalid InstructorId");

        // Prevent duplicate timetable for the same instructor
        var existing = await _context.Timetables
            .FirstOrDefaultAsync(t => t.InstructorId == timetable.InstructorId);
        if (existing != null)
            return Conflict("This instructor already has a timetable");

        _context.Timetables.Add(timetable);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = timetable.IdTimetable }, timetable);
    }

    // PUT: api/timetable/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Timetable timetable)
    {
        if (id != timetable.IdTimetable) return BadRequest();

        var existing = await _context.Timetables.AsNoTracking().FirstOrDefaultAsync(t => t.IdTimetable == id);
        if (existing == null) return NotFound();

        // Optional: validate instructor exists
        var instructorExists = await _context.People
            .OfType<Instructor>()
            .AnyAsync(i => i.IdPerson == timetable.InstructorId);
        if (!instructorExists)
            return BadRequest("Invalid InstructorId");

        _context.Entry(timetable).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/timetable/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var timetable = await _context.Timetables.FindAsync(id);
        if (timetable == null) return NotFound();

        _context.Timetables.Remove(timetable);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // GET timetable by Instructor LicenseNo (qualifier)
    [HttpGet("by-license/{licenseNo}")]
    public async Task<ActionResult<Timetable>> GetByInstructorLicense(string licenseNo)
    {
        var instructor = await _context.People
            .OfType<Instructor>()
            .FirstOrDefaultAsync(i => i.LicenseNo == licenseNo);
        if (instructor == null) return NotFound("Instructor not found");

        var timetable = await _context.Timetables
            .Include(t => t.Timeslots)
            .FirstOrDefaultAsync(t => t.InstructorId == instructor.IdPerson);

        if (timetable == null) return NotFound("Timetable not found");
        return timetable;
    }

    // CREATE timetable for instructor identified by LicenseNo
    [HttpPost("for-instructor/{licenseNo}")]
    public async Task<ActionResult<Timetable>> CreateForInstructor(string licenseNo, Timetable newTimetable)
    {
        var instructor = await _context.People
            .OfType<Instructor>()
            .FirstOrDefaultAsync(i => i.LicenseNo == licenseNo);
        if (instructor == null)
            return NotFound("Instructor not found");

        var existing = await _context.Timetables
            .FirstOrDefaultAsync(t => t.InstructorId == instructor.IdPerson);
        if (existing != null)
            return Conflict("This instructor already has a timetable");

        newTimetable.InstructorId = instructor.IdPerson;
        _context.Timetables.Add(newTimetable);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetByInstructorLicense), new { licenseNo }, newTimetable);
    }

    // DELETE timetable for instructor identified by LicenseNo
    [HttpDelete("for-instructor/{licenseNo}")]
    public async Task<IActionResult> DeleteForInstructor(string licenseNo)
    {
        var instructor = await _context.People
            .OfType<Instructor>()
            .FirstOrDefaultAsync(i => i.LicenseNo == licenseNo);
        if (instructor == null) return NotFound("Instructor not found");

        var timetable = await _context.Timetables
            .FirstOrDefaultAsync(t => t.InstructorId == instructor.IdPerson);
        if (timetable == null) return NotFound("Timetable not found");

        _context.Timetables.Remove(timetable);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpGet("with-instructors")]
    public async Task<ActionResult<IEnumerable<Timetable>>> GetAllWithInstructors()
    {
        return await _context.Timetables
            .Include(t => t.Timeslots)
            .Include(t => t.Instructor)   // assuming navigation property exists
            .ToListAsync();
    }
    [HttpGet("by-date/{date}")]
    public async Task<ActionResult<IEnumerable<Timetable>>> GetByDate(DateTime date)
    {
        var timetables = await _context.Timetables
            .Include(t => t.Timeslots)
            .Where(t => t.Timeslots.Any(ts => ts.Date.Date == date.Date))
            .ToListAsync();

        return timetables;
    }
    [HttpPost("{timetableId}/timeslots")]
    public async Task<ActionResult<Timeslot>> AddTimeslot(int timetableId, Timeslot newTimeslot)
    {
        var timetable = await _context.Timetables
            .Include(t => t.Timeslots)
            .FirstOrDefaultAsync(t => t.IdTimetable == timetableId);

        if (timetable == null) return NotFound();

        timetable.Timeslots.Add(newTimeslot);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = timetableId }, newTimeslot);
    }
    [HttpGet("summary")]
    public async Task<ActionResult<IEnumerable<object>>> GetTimetableSummary()
    {
        var data = await _context.Timetables
            .Select(t => new {
                t.IdTimetable,
                InstructorName = t.Instructor.Name, // jeśli jest nawigacja
                TimeslotCount = t.Timeslots.Count
            }).ToListAsync();

        return Ok(data);
    }
    [HttpDelete("{timetableId}/timeslots/{timeslotId}")]
    public async Task<IActionResult> DeleteTimeslotFromTimetable(int timetableId, int timeslotId)
    {
        // Step 1: Retrieve the timeslot
        var timeslot = await _context.Timeslots
            .FirstOrDefaultAsync(ts => ts.IdTimeslot == timeslotId && ts.IdTimetable == timetableId);

        if (timeslot == null)
        {
            return NotFound("Timeslot not found or not part of the specified timetable.");
        }

        // Step 2: Remove the timeslot
        _context.Timeslots.Remove(timeslot);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    

}
