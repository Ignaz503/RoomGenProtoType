using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VirtMuseWeb.Models;

namespace VirtMuseWeb.Pages.Resources.MetaDataCreators
{
    public class DetailsModel : PageModel
    {
        private readonly VirtMuseWeb.Models.VirtMuseWebContext _context;

        public DetailsModel(VirtMuseWeb.Models.VirtMuseWebContext context)
        {
            _context = context;
        }

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
            return Page();
        }
    }
}
