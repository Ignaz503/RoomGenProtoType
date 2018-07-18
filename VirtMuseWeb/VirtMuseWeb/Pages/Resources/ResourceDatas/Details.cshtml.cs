using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VirtMuseWeb.Models;

namespace VirtMuseWeb.Pages.Resources.ResourceDatas
{
    public class DetailsModel : PageModel
    {
        private readonly VirtMuseWeb.Models.VirtMuseWebContext _context;

        public DetailsModel(VirtMuseWeb.Models.VirtMuseWebContext context)
        {
            _context = context;
        }

        public ResourceData ResourceData { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ResourceData = await _context.ResourceData
                .Include(r => r.Resource).SingleOrDefaultAsync(m => m.ID == id);

            if (ResourceData == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
