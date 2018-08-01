using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace VirtMuseWeb.Models
{
    public class VirtMuseWebContext : DbContext
    {
        public VirtMuseWebContext (DbContextOptions<VirtMuseWebContext> options)
            : base(options)
        {
        }

        //public DbSet<VirtMuseWeb.Models.Resource> Resource { get; set; }

        //public DbSet<VirtMuseWeb.Models.MetaData> MetaData { get; set; }
        //public DbSet<VirtMuseWeb.Models.Creator> Creator { get; set; }
        //public DbSet<VirtMuseWeb.Models.Source> Source { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
