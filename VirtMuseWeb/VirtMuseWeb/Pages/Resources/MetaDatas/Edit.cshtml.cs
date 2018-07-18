using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VirtMuseWeb.Models;

namespace VirtMuseWeb.Pages.Resources.MetaDatas
{
    public class EditModel : PageModel
    {
        private readonly VirtMuseWeb.Models.VirtMuseWebContext _context;

        public EditModel(VirtMuseWeb.Models.VirtMuseWebContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MetaData MetaData { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MetaData = await _context.MetaData
                .Include(m => m.Resource).SingleOrDefaultAsync(m => m.ID == id);

            if (MetaData == null)
            {
                return NotFound();
            }
           ViewData["ID"] = new SelectList(_context.Resource, "ID", "ID");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(MetaData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MetaDataExists(MetaData.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool MetaDataExists(int id)
        {
            return _context.MetaData.Any(e => e.ID == id);
        }
    }
}
