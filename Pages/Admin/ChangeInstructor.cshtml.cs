using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using YinStudio.Models;

namespace YinStudio.Pages.Admin
{
    public class ChangeInstructorModel : PageModel
    {
        private readonly YinStudioContext _context;

        public ChangeInstructorModel(YinStudioContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int TimeslotId { get; set; }

        public Timeslot Timeslot { get; set; }

        public List<Instructor> Instructors { get; set; }

        [BindProperty]
        public int SelectedInstructorId { get; set; }

        public string Message { get; set; }

        // Fetch instructors exactly like you fetch customers previously
        private async Task<List<Instructor>> GetAllInstructorsAsync()
        {
            return await _context.People
                .OfType<Instructor>()
                .ToListAsync();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Timeslot = await _context.Timeslots
                .Include(t => t.Timetable)
                    .ThenInclude(tt => tt.Instructor)
                .FirstOrDefaultAsync(t => t.IdTimeslot == TimeslotId);

            if (Timeslot == null)
                return NotFound();

            Instructors = await GetAllInstructorsAsync();

            SelectedInstructorId = Timeslot.Timetable?.InstructorId ?? 0;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Timeslot = await _context.Timeslots
                .Include(t => t.Timetable)
                    .ThenInclude(tt => tt.Instructor)
                .FirstOrDefaultAsync(t => t.IdTimeslot == TimeslotId);

            if (Timeslot == null)
                return NotFound();

            var instructor = await _context.People
                .OfType<Instructor>()
                .FirstOrDefaultAsync(i => i.IdPerson == SelectedInstructorId);

            if (instructor == null)
            {
                Message = "Selected instructor not found.";
                Instructors = await GetAllInstructorsAsync();
                return Page();
            }

            if (instructor.Timetable == null)
            {
                instructor.Timetable = new Timetable
                {
                    InstructorId = instructor.IdPerson,
                    Version = "v1"
                };
                _context.Timetables.Add(instructor.Timetable);
                await _context.SaveChangesAsync();
            }

            Timeslot.IdTimetable = instructor.Timetable.IdTimetable;
            await _context.SaveChangesAsync();

            Message = $"Instructor {instructor.Name} {instructor.Surname} assigned to this timeslot.";

            Instructors = await GetAllInstructorsAsync();

            return Page();
        }
    }
}
