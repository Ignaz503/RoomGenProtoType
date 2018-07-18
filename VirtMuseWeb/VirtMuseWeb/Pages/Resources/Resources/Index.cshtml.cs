using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VirtMuseWeb.Models;

namespace VirtMuseWeb.Pages.Resources.Resources
{
    public class IndexModel : PageModel
    {
        private readonly VirtMuseWeb.Models.VirtMuseWebContext _context;

        public IndexModel(VirtMuseWeb.Models.VirtMuseWebContext context)
        {
            _context = context;
        }

        public IList<Resource> Resource { get;set; }

        public async Task OnGetAsync()
        {
            Resource = await _context.Resource.ToListAsync();
        }
    }
}
