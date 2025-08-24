using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YinStudio.Models;

namespace YinStudio.Pages.Admin
{
   public class DashboardModel : PageModel
{
    private readonly YinStudioContext _context;

    public DashboardModel(YinStudioContext context)
    {
        _context = context;
    }

    public List<Timeslot> TimeslotsToday { get; set; } = new();
    public List<Instructor> Instructors { get; set; } = new();

    [BindProperty]
    public int SelectedInstructorId { get; set; }

    [BindProperty]
    public int SelectedTimeslotId { get; set; }

    public string Message { get; set; } = "";

    public async Task OnGetAsync()
    {
        var today = DateTime.Today;

        TimeslotsToday = await _context.Timeslots
            .Include(t => t.Timetable)
                .ThenInclude(tt => tt.Instructor)
            .Where(t => t.Date == today)
            .ToListAsync();

        Instructors = await _context.People.OfType<Instructor>()
            .Where(i => i.Available == true)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAssignAsync()
    {
        if (SelectedTimeslotId == 0 || SelectedInstructorId == 0)
        {
            Message = "Please select a timeslot and instructor.";
            await OnGetAsync();
            return Page();
        }

        var timeslot = await _context.Timeslots
            .Include(t => t.Timetable)
            .FirstOrDefaultAsync(t => t.IdTimeslot == SelectedTimeslotId);

        var instructor = await _context.People
            .OfType<Instructor>()
            .Include(i => i.Timetable)
            .FirstOrDefaultAsync(i => i.IdPerson == SelectedInstructorId);

        if (timeslot == null || instructor == null)
        {
            Message = "Invalid timeslot or instructor selected.";
            await OnGetAsync();
            return Page();
        }

        // Ensure instructor has a timetable; create if not
        if (instructor.Timetable == null)
        {
            instructor.Timetable = new Timetable
            {
                InstructorId = instructor.IdPerson,
                Version = "v1" // You can update this version as needed
            };
            _context.Timetables.Add(instructor.Timetable);
            await _context.SaveChangesAsync();
        }

        // Assign timeslot to instructor's timetable
        timeslot.IdTimetable = instructor.Timetable.IdTimetable;

        await _context.SaveChangesAsync();

        Message = $"Instructor {instructor.Name} assigned to timeslot {timeslot.HourFrom} - {timeslot.HourTo}.";

        await OnGetAsync();
        return Page();
    }
}

}
