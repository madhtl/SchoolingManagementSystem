using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YinStudio.Models;

namespace YinStudio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimeslotController : ControllerBase
{
    private readonly YinStudioContext _context;

    public TimeslotController(YinStudioContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Timeslot>>> GetAll()
    {
        return await _context.Timeslots.Include(ts => ts.Timetable).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Timeslot>> GetById(int id)
    {
        var timeslot = await _context.Timeslots
            .Include(ts => ts.Timetable)
            .FirstOrDefaultAsync(ts => ts.IdTimeslot == id);

        if (timeslot == null) return NotFound();
        return timeslot;
    }

    [HttpPost]
    public async Task<ActionResult<Timeslot>> Create(Timeslot timeslot)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Timeslots.Add(timeslot);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = timeslot.IdTimeslot }, timeslot);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Timeslot timeslot)
    {
        if (id != timeslot.IdTimeslot)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Entry(timeslot).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var timeslot = await _context.Timeslots.FindAsync(id);
        if (timeslot == null) return NotFound();

        _context.Timeslots.Remove(timeslot);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}