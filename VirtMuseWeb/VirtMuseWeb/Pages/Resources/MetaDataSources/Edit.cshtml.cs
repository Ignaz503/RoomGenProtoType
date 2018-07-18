using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VirtMuseWeb.Models;

namespace VirtMuseWeb.Pages.Resources.MetaDataSources
{
    public class EditModel : PageModel
    {
        private readonly VirtMuseWeb.Models.VirtMuseWebContext _context;

        public EditModel(VirtMuseWeb.Models.VirtMuseWebContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MetaDataSource MetaDataSource { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MetaDataSource = await _context.MetaDataSource
                .Include(m => m.MetaData)
                .Include(m => m.Source).SingleOrDefaultAsync(m => m.MetaDataID == id);

            if (MetaDataSource == null)
            {
                return NotFound();
            }
           ViewData["MetaDataID"] = new SelectList(_context.MetaData, "ID", "ID");
           ViewData["SourceID"] = new SelectList(_context.Source, "ID", "ID");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(MetaDataSource).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MetaDataSourceExists(MetaDataSource.MetaDataID))
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

        private bool MetaDataSourceExists(int id)
        {
            return _context.MetaDataSource.Any(e => e.MetaDataID == id);
        }
    }
}
