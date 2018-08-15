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

        public DbSet<ResourceModel> Resource { get; set; }
        //public DbSet<Creator> Creator { get; set; }
        //public DbSet<Source> Source { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //comented out
            #region Table splitting more advaanced data model
            //table splitting resource metadata
            //modelBuilder.Entity<ResourceModel>().HasKey(x => x.ID);
            //modelBuilder.Entity<ResourceModel>().HasOne(m => m.MetaData).WithOne(r => r.Resource).HasForeignKey<MetaDataModel>(m => m.ResourceID);
            //modelBuilder.Entity<ResourceModel>().ToTable("Resource");

            //modelBuilder.Entity<MetaDataModel>().HasKey(m => m.ResourceID);
            //modelBuilder.Entity<MetaDataModel>().HasOne(r => r.Resource).WithOne(m => m.MetaData).HasForeignKey<ResourceModel>(r => r.ID);
            //modelBuilder.Entity<MetaDataModel>().ToTable("Resource");
            #endregion
        }
    }
}
