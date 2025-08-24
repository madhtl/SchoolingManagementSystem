using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using YinStudio.Models;

namespace YinStudio.Pages.Admin
{
    public class ClassesModel : PageModel
    {
        private readonly YinStudioContext _context;

        public ClassesModel(YinStudioContext context)
        {
            _context = context;
        }

        public List<Class> Classes { get; set; }

        public async Task OnGetAsync()
        {
            Classes = await _context.Classes.Include(c => c.Instructor).ToListAsync();
        }
    }
}
