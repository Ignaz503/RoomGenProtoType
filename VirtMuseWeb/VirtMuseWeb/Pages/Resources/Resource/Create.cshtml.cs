using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VirtMuseWeb.Models;

namespace VirtMuseWeb.Pages.Resources.Resource
{
    public class CreateModel : PageModel
    {
        private readonly VirtMuseWeb.Models.VirtMuseWebContext _context;

        public SelectList ResTypeSelectList = new SelectList(Enum.GetValues(typeof(ResourceType)));

        public CreateModel(VirtMuseWeb.Models.VirtMuseWebContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Models.Resource Resource { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            Resource.PostTime = DateTime.Now;
            Resource.User = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Resource.Add(Resource);
            await _context.SaveChangesAsync();

            return RedirectToPage("../MetaData/Create");
        }
    }
}