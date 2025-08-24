using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YinStudio.Models;

namespace YinStudio.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExpController : ControllerBase
{
    private readonly YinStudioContext _context;

    public ExpController(YinStudioContext context)
    {
        _context = context;
    }

    // GET: api/Exps
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Exp>>> GetExps()
    {
        return await _context.Exps
            .Include(e => e.Membership)
            .ToListAsync();
    }

    // GET: api/Exps/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Exp>> GetExp(int id)
    {
        var exp = await _context.Exps
            .Include(e => e.Membership)
            .FirstOrDefaultAsync(e => e.IdExp == id);

        if (exp == null)
        {
            return NotFound();
        }

        return exp;
    }

    [HttpPost]
    public async Task<ActionResult<Exp>> CreateExp(Exp exp)
    {
        // Check for duplicate: same membership and expiration date
        bool duplicateExists = await _context.Exps.AnyAsync(e =>
            e.IdMembership == exp.IdMembership &&
            e.ExpirationDate == exp.ExpirationDate);

        if (duplicateExists)
            return Conflict("An Exp with the same Membership and ExpirationDate already exists.");

        _context.Exps.Add(exp);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetExp), new { id = exp.IdExp }, exp);
    }

    // PUT: api/Exps/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExp(int id, Exp updatedExp)
    {
        if (id != updatedExp.IdExp)
            return BadRequest("ID mismatch");

        var existingExp = await _context.Exps.FindAsync(id);
        if (existingExp == null)
            return NotFound();

        existingExp.Description = updatedExp.Description;
        existingExp.ExpirationDate = updatedExp.ExpirationDate;
        existingExp.IdMembership = updatedExp.IdMembership;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Exps/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExp(int id)
    {
        var exp = await _context.Exps.FindAsync(id);
        if (exp == null)
        {
            return NotFound();
        }

        _context.Exps.Remove(exp);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
