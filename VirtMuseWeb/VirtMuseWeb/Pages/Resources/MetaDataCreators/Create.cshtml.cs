using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VirtMuseWeb.Models;

namespace VirtMuseWeb.Pages.Resources.MetaDataCreators
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
        ViewData["CreatorID"] = new SelectList(_context.Creator, "ID", "ID");
        ViewData["MetaDataID"] = new SelectList(_context.MetaData, "ID", "ID");
            return Page();
        }

        [BindProperty]
        public MetaDataCreator MetaDataCreator { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.MetaDataCreator.Add(MetaDataCreator);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}