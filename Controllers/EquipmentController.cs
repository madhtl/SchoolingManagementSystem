using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YinStudio.Models;

namespace YinStudio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipmentController : ControllerBase
{
    private readonly YinStudioContext _context;

    public EquipmentController(YinStudioContext context)
    {
        _context = context;
    }

    // GET: api/Equipment
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Equipment>>> GetAll()
    {
        // Returns all equipment without related Onsite classes for brevity
        return await _context.Equipment.ToListAsync();
    }

    // GET: api/Equipment/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Equipment>> GetById(int id)
    {
        var equipment = await _context.Equipment
            .Include(e => e.Onsite) // Load related Onsite classes (many-to-many)
            .FirstOrDefaultAsync(e => e.IdEquipment == id);

        if (equipment == null)
            return NotFound();

        return equipment;
    }

    // POST: api/Equipment
    [HttpPost]
    public async Task<ActionResult<Equipment>> Create(Equipment equipment)
    {
        _context.Equipment.Add(equipment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = equipment.IdEquipment }, equipment);
    }

    // PUT: api/Equipment/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Equipment equipment)
    {
        if (id != equipment.IdEquipment)
            return BadRequest();

        _context.Entry(equipment).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await EquipmentExists(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    // DELETE: api/Equipment/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var equipment = await _context.Equipment.FindAsync(id);

        if (equipment == null)
            return NotFound();

        _context.Equipment.Remove(equipment);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> EquipmentExists(int id)
    {
        return await _context.Equipment.AnyAsync(e => e.IdEquipment == id);
    }
}
