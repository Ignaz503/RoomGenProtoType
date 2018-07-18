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
    public class IndexModel : PageModel
    {
        private readonly VirtMuseWeb.Models.VirtMuseWebContext _context;

        public IndexModel(VirtMuseWeb.Models.VirtMuseWebContext context)
        {
            _context = context;
        }

        public IList<MetaDataCreator> MetaDataCreator { get;set; }

        public async Task OnGetAsync()
        {
            MetaDataCreator = await _context.MetaDataCreator
                .Include(m => m.Creator)
                .Include(m => m.MetaData).ToListAsync();
        }
    }
}
