using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YinStudio.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace YinStudio.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MembershipsController : ControllerBase
{
    private readonly YinStudioContext _dbContext;

    public MembershipsController(YinStudioContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet(Name = "GetAllMemberships")]
    public async Task<IActionResult> GetMemberships()
    {
        var memberships = await _dbContext.Memberships
            .Include(m => m.Exps)
            .ToListAsync();

        return Ok(memberships);
    }

    [HttpGet("{id}", Name = "GetMembershipById")]
    public async Task<IActionResult> GetMembership(int id)
    {
        var membership = await _dbContext.Memberships
            .Include(m => m.Exps)
            .FirstOrDefaultAsync(m => m.IdMembership == id);

        if (membership == null)
            return NotFound();

        return Ok(membership);
    }

    [HttpPost(Name = "CreateMembership")]
    public async Task<IActionResult> CreateMembership([FromBody] Membership membership)
    {
        _dbContext.Memberships.Add(membership);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMembership), new { id = membership.IdMembership }, membership);
    }

    [HttpPut("{id}", Name = "UpdateMembership")]
    public async Task<IActionResult> UpdateMembership(int id, [FromBody] Membership updatedMembership)
    {
        if (id != updatedMembership.IdMembership)
            return BadRequest("ID mismatch");

        var existingMembership = await _dbContext.Memberships.FindAsync(id);
        if (existingMembership == null)
            return NotFound();

        existingMembership.Type = updatedMembership.Type;
        existingMembership.Price = updatedMembership.Price;

        await _dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}", Name = "DeleteMembership")]
    public async Task<IActionResult> DeleteMembership(int id)
    {
        var membership = await _dbContext.Memberships.FindAsync(id);
        if (membership == null)
            return NotFound();

        _dbContext.Memberships.Remove(membership);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
    // GET: api/Memberships/byType/{type}
    [HttpGet("byType/{type}", Name = "GetMembershipsByType")]
    public async Task<IActionResult> GetMembershipsByType(MembershipType type)
    {
        var memberships = await _dbContext.Memberships
            .Where(m => m.Type == type)
            .Include(m => m.Exps)
            .ToListAsync();

        if (!memberships.Any())
            return NotFound($"No memberships found with type '{type}'.");

        return Ok(memberships);
    }


}
