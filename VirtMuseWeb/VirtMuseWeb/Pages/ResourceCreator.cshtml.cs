using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VirtMuseWeb.Models;

namespace VirtMuseWeb.Pages
{
    public class ResourceCreatorModel : PageModel
    {

        private readonly VirtMuseWeb.Models.VirtMuseWebContext _context;

        public  ResourceCreatorModel(VirtMuseWeb.Models.VirtMuseWebContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ResourceBinding resinding { get; set; }

        public void OnGet()
        {

        }
    }
}