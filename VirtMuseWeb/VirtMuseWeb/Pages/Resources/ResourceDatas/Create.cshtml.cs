using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VirtMuseWeb.Models;

namespace VirtMuseWeb.Pages.Resources.ResourceDatas
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
        ViewData["ResourceID"] = new SelectList(_context.Resource, "ID", "ID");
            return Page();
        }

        [BindProperty]
        public ResourceData ResourceData { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ResourceData.Add(ResourceData);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}