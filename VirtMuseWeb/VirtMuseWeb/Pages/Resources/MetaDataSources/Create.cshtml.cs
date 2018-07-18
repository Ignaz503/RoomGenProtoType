using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VirtMuseWeb.Models;

namespace VirtMuseWeb.Pages.Resources.MetaDataSources
{
    public class CreateModel : PageModel
    {
        private readonly VirtMuseWeb.Models.VirtMuseWebContext _context;

        public CreateModel(VirtMuseWeb.Models.VirtMuseWebContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["MetaDataID"] = new SelectList(_context.MetaData, "ID", "ID");
        ViewData["SourceID"] = new SelectList(_context.Source, "ID", "ID");
            return Page();
        }

        [BindProperty]
        public MetaDataSource MetaDataSource { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.MetaDataSource.Add(MetaDataSource);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}