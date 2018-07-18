using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VirtMuseWeb.Models;

namespace VirtMuseWeb.Pages.Resources.MetaDataSources
{
    public class DeleteModel : PageModel
    {
        private readonly VirtMuseWeb.Models.VirtMuseWebContext _context;

        public DeleteModel(VirtMuseWeb.Models.VirtMuseWebContext context)
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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MetaDataSource = await _context.MetaDataSource.FindAsync(id);

            if (MetaDataSource != null)
            {
                _context.MetaDataSource.Remove(MetaDataSource);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
