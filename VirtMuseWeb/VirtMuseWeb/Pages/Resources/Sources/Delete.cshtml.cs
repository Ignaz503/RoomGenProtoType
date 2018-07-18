using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VirtMuseWeb.Models;

namespace VirtMuseWeb.Pages.Resources.Sources
{
    public class DeleteModel : PageModel
    {
        private readonly VirtMuseWeb.Models.VirtMuseWebContext _context;

        public DeleteModel(VirtMuseWeb.Models.VirtMuseWebContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Source Source { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Source = await _context.Source.SingleOrDefaultAsync(m => m.ID == id);

            if (Source == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Source = await _context.Source.FindAsync(id);

            if (Source != null)
            {
                _context.Source.Remove(Source);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
