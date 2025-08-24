using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YinStudio.Models;

namespace YinStudio.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly YinStudioContext _context;

    public ReviewController(YinStudioContext context)
    {
        _context = context;
    }

    // GET: api/review
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Review>>> GetReviews()
    {
        return await _context.Set<Review>()
            .Include(r => r.Customer)
            .Include(r => r.Class)
            .ToListAsync();
    }

    // GET: api/review/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Review>> GetReview(int id)
    {
        var review = await _context.Set<Review>()
            .Include(r => r.Customer)
            .Include(r => r.Class)
            .FirstOrDefaultAsync(r => r.IdReview == id);

        if (review == null)
            return NotFound();

        return review;
    }

    [HttpPost]
    public async Task<ActionResult<Review>> CreateReview(Review review)
    {
        // Ensure the customer has actually ordered the class
        var orderExists = await _context.Orders
            .AnyAsync(o => o.CustomerId == review.IdCustomer && o.ClassId == review.IdClass);

        if (!orderExists)
            return BadRequest("Customer must have an order for the class before posting a review.");

        // Check for duplicate review by the same customer for the same class
        var reviewExists = await _context.Set<Review>()
            .AnyAsync(r => r.IdCustomer == review.IdCustomer && r.IdClass == review.IdClass);

        if (reviewExists)
            return Conflict("Customer has already reviewed this class.");

        _context.Set<Review>().Add(review);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetReview), new { id = review.IdReview }, review);
    }


    // PUT: api/review/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(int id, Review updatedReview)
    {
        if (id != updatedReview.IdReview)
            return BadRequest("ID mismatch.");

        var existingReview = await _context.Set<Review>().FindAsync(id);
        if (existingReview == null)
            return NotFound();

        var orderExists = await _context.Orders
            .AnyAsync(o => o.CustomerId == updatedReview.IdCustomer && o.ClassId == updatedReview.IdClass);

        if (!orderExists)
            return BadRequest("Cannot assign review to a class the customer hasn't ordered.");

        _context.Entry(existingReview).CurrentValues.SetValues(updatedReview);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/review/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var review = await _context.Set<Review>().FindAsync(id);
        if (review == null)
            return NotFound();

        _context.Set<Review>().Remove(review);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    // GET: api/review/byClass/5
    [HttpGet("byClass/{classId}")]
    public async Task<ActionResult<IEnumerable<Review>>> GetReviewsByClass(int classId)
    {
        var reviews = await _context.Set<Review>()
            .Where(r => r.IdClass == classId)
            .Include(r => r.Customer)
            .ToListAsync();

        if (!reviews.Any())
            return NotFound($"No reviews found for class ID {classId}.");

        return Ok(reviews);
    }
    // GET: api/review/byCustomer/5
    [HttpGet("byCustomer/{customerId}")]
    public async Task<ActionResult<IEnumerable<Review>>> GetReviewsByCustomer(int customerId)
    {
        var reviews = await _context.Set<Review>()
            .Where(r => r.IdCustomer == customerId)
            .Include(r => r.Class)
            .ToListAsync();

        if (!reviews.Any())
            return NotFound($"No reviews found from customer ID {customerId}.");

        return Ok(reviews);
    }
    // GET: api/review/classAverage/5
    [HttpGet("classAverage/{classId}")]
    public async Task<ActionResult<double>> GetAverageRankingForClass(int classId)
    {
        var reviews = await _context.Set<Review>()
            .Where(r => r.IdClass == classId)
            .ToListAsync();

        if (!reviews.Any())
            return NotFound($"No reviews found for class ID {classId}.");

        var average = reviews.Average(r => r.Ranking);
        return Ok(Math.Round(average, 2));
    }



}
