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

        public DbSet<VirtMuseWeb.Models.Resource> Resource { get; set; }
        public DbSet<VirtMuseWeb.Models.ResourceData> ResourceData { get; set; }
        public DbSet<VirtMuseWeb.Models.MetaDataCreator> MetaDataCreator { get; set; }
        public DbSet<VirtMuseWeb.Models.MetaData> MetaData { get; set; }
        public DbSet<VirtMuseWeb.Models.Creator> Creator { get; set; }
        public DbSet<VirtMuseWeb.Models.Source> Source { get; set; }
        public DbSet<VirtMuseWeb.Models.MetaDataSource> MetaDataSource { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //resource metadata one to one

            modelBuilder.Entity<Resource>().HasOne(r => r.MetaData).WithOne(m => m.Resource).HasForeignKey<Resource>(r => r.ID);

            modelBuilder.Entity<MetaData>().HasOne(m => m.Resource).WithOne(r => r.MetaData).HasForeignKey<MetaData>(m => m.ID);

            //MetaDataCreator m to n join table
            modelBuilder.Entity<MetaDataCreator>().HasKey(mk => new { mk.MetaDataID, mk.CreatorID });

            //modelBuilder.Entity<MetaDataCreator>().HasOne(mk => mk.MetaData).WithMany(m => m.Creators).HasForeignKey(mc => mc.MetaDataID);

            //modelBuilder.Entity<MetaDataCreator>().HasOne(ck => ck.Creator).WithMany(c => c.PiecesCreated).HasForeignKey(ck => ck.CreatorID);


            //METADATASOURCE m to n relation ship join table
            modelBuilder.Entity<MetaDataSource>().HasKey(mk => new { mk.MetaDataID, mk.SourceID });

            //modelBuilder.Entity<MetaDataSource>().HasOne(mk => mk.MetaData).WithMany(m => m.Sources).HasForeignKey(mk => mk.MetaDataID);

            //modelBuilder.Entity<MetaDataSource>().HasOne(sk => sk.Source).WithMany(s => s.SourceFor).HasForeignKey(sk => sk.SourceID);
        }
    }
}
