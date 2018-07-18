using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VirtMuseWeb.Models;

namespace VirtMuseWeb.Pages.Resources.Creators
{
    public class DeleteModel : PageModel
    {
        private readonly VirtMuseWeb.Models.VirtMuseWebContext _context;

        public DeleteModel(VirtMuseWeb.Models.VirtMuseWebContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Creator Creator { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Creator = await _context.Creator.SingleOrDefaultAsync(m => m.ID == id);

            if (Creator == null)
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

            Creator = await _context.Creator.FindAsync(id);

            if (Creator != null)
            {
                _context.Creator.Remove(Creator);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
