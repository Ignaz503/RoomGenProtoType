using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VirtMuseWeb.Models;

namespace VirtMuseWeb.Pages.Resources.Sources
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
            return Page();
        }

        [BindProperty]
        public Source Source { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Source.Add(Source);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}