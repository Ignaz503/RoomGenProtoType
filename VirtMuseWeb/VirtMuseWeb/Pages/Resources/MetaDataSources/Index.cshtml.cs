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
    public class IndexModel : PageModel
    {
        private readonly VirtMuseWeb.Models.VirtMuseWebContext _context;

        public IndexModel(VirtMuseWeb.Models.VirtMuseWebContext context)
        {
            _context = context;
        }

        public IList<MetaDataSource> MetaDataSource { get;set; }

        public async Task OnGetAsync()
        {
            MetaDataSource = await _context.MetaDataSource
                .Include(m => m.MetaData)
                .Include(m => m.Source).ToListAsync();
        }
    }
}
