using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VirtMuseWeb.Models;

namespace VirtMuseWeb.Pages.Resources.MetaDataCreators
{
    public class EditModel : PageModel
    {
        private readonly VirtMuseWeb.Models.VirtMuseWebContext _context;

        public EditModel(VirtMuseWeb.Models.VirtMuseWebContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MetaDataCreator MetaDataCreator { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MetaDataCreator = await _context.MetaDataCreator
                .Include(m => m.Creator)
                .Include(m => m.MetaData).SingleOrDefaultAsync(m => m.MetaDataID == id);

            if (MetaDataCreator == null)
            {
                return NotFound();
            }
           ViewData["CreatorID"] = new SelectList(_context.Creator, "ID", "ID");
           ViewData["MetaDataID"] = new SelectList(_context.MetaData, "ID", "ID");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(MetaDataCreator).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MetaDataCreatorExists(MetaDataCreator.MetaDataID))
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

        private bool MetaDataCreatorExists(int id)
        {
            return _context.MetaDataCreator.Any(e => e.MetaDataID == id);
        }
    }
}
