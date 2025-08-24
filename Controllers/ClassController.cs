using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YinStudio.Models;

namespace YinStudio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassController : ControllerBase
    {
        private readonly YinStudioContext _context;

        public ClassController(YinStudioContext context)
        {
            _context = context;
        }

        // GET: api/Class
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Class>>> GetClasses()
        {
            return await _context.Classes.ToListAsync();
        }

        // GET: api/Class/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Class>> GetClass(int id)
        {
            var cls = await _context.Classes.FindAsync(id);

            if (cls == null)
            {
                return NotFound();
            }

            return cls;
        }

        // POST: api/Class
        [HttpPost]
        public async Task<ActionResult<Class>> PostClass(Class cls)
        {
            _context.Classes.Add(cls);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClass), new { id = cls.IdClass }, cls);
        }

        // PUT: api/Class/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClass(int id, Class cls)
        {
            if (id != cls.IdClass)
                return BadRequest();

            _context.Entry(cls).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClassExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Class/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var cls = await _context.Classes.FindAsync(id);
            if (cls == null)
                return NotFound();

            _context.Classes.Remove(cls);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Class/5/customers
        [HttpGet("{id}/customers")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomersForClass(int id)
        {
            var cls = await _context.Classes
                .Include(c => c.Customers)
                .FirstOrDefaultAsync(c => c.IdClass == id);

            if (cls == null)
                return NotFound();

            return Ok(cls.Customers);
        }

        // POST: api/Class/5/sign-customer/10
        [HttpPost("{classId}/sign-customer/{personId}")]
        public async Task<IActionResult> SignCustomerToClass(int classId, int personId)
        {
            var cls = await _context.Classes
                .Include(c => c.Customers)
                .FirstOrDefaultAsync(c => c.IdClass == classId);

            if (cls == null)
                return NotFound($"Class with ID {classId} not found.");

            var person = await _context.People
                .OfType<Customer>()
                .FirstOrDefaultAsync(p => p.IdPerson == personId);

            if (person == null)
                return NotFound($"Customer with ID {personId} not found (or not a Customer).");

            if (cls.Customers.Any(c => c.IdPerson == personId))
                return BadRequest("Customer is already signed up for this class.");

            cls.Customers.Add(person);
            await _context.SaveChangesAsync();

            return Ok($"Customer {personId} successfully signed to class {classId}.");
        }

        // GET: api/Class/5/equipment
        [HttpGet("{classId}/equipment")]
        public async Task<ActionResult<IEnumerable<Equipment>>> GetEquipmentForClass(int classId)
        {
            // We only expect Onsite classes to have equipment
            var onsiteClass = await _context.Classes
                .OfType<Onsite>()
                .Include(o => o.EquipmentAvailable)
                .FirstOrDefaultAsync(c => c.IdClass == classId);

            if (onsiteClass == null)
                return NotFound($"Onsite class with ID {classId} not found.");

            return Ok(onsiteClass.EquipmentAvailable);
        }

        // POST: api/Class/5/add-equipment/10
        [HttpPost("{classId}/add-equipment/{equipmentId}")]
        public async Task<IActionResult> AddEquipmentToClass(int classId, int equipmentId)
        {
            var onsiteClass = await _context.Classes
                .OfType<Onsite>()
                .Include(o => o.EquipmentAvailable)
                .FirstOrDefaultAsync(c => c.IdClass == classId);

            if (onsiteClass == null)
                return NotFound($"Onsite class with ID {classId} not found.");

            var equipment = await _context.Equipment.FindAsync(equipmentId);
            if (equipment == null)
                return NotFound($"Equipment with ID {equipmentId} not found.");

            if (onsiteClass.EquipmentAvailable.Any(e => e.IdEquipment == equipmentId))
                return BadRequest("Equipment is already assigned to this class.");

            onsiteClass.EquipmentAvailable.Add(equipment);
            await _context.SaveChangesAsync();

            return Ok($"Equipment {equipmentId} assigned to class {classId}.");
        }

        // DELETE: api/Class/5/remove-equipment/10
        [HttpDelete("{classId}/remove-equipment/{equipmentId}")]
        public async Task<IActionResult> RemoveEquipmentFromClass(int classId, int equipmentId)
        {
            var onsiteClass = await _context.Classes
                .OfType<Onsite>()
                .Include(o => o.EquipmentAvailable)
                .FirstOrDefaultAsync(c => c.IdClass == classId);

            if (onsiteClass == null)
                return NotFound($"Onsite class with ID {classId} not found.");

            var equipment = onsiteClass.EquipmentAvailable.FirstOrDefault(e => e.IdEquipment == equipmentId);
            if (equipment == null)
                return NotFound($"Equipment with ID {equipmentId} is not assigned to this class.");

            onsiteClass.EquipmentAvailable.Remove(equipment);
            await _context.SaveChangesAsync();

            return Ok($"Equipment {equipmentId} removed from class {classId}.");
        }

        private bool ClassExists(int id)
        {
            return _context.Classes.Any(e => e.IdClass == id);
        }
    }
}
