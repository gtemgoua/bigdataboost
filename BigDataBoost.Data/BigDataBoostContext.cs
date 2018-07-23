using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BigDataBoost.Model;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BigDataBoost.Data
{
    public class BigDataBoostContext : DbContext
    {
        public DbSet<TagDef> Tags { get; set; }
        public DbSet<TagHist> Historian { get; set; }

        public BigDataBoostContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            /// Model Building for TagDef Table
            modelBuilder.Entity<TagDef>()
                .ToTable("TagDef");

            modelBuilder.Entity<TagDef>()
                .Property(s => s.Source)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<TagDef>()
                .Property(s => s.Name)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<TagDef>()
                .Property(s => s.Description)
                .HasMaxLength(100);

            modelBuilder.Entity<TagDef>()
                .Property(s => s.ExtendedDescription)
                .HasMaxLength(100);

            //modelBuilder.Entity<TagDef>()
            //    .Property(s => s.Value)
            //    .HasDefaultValue(0.0);

            //modelBuilder.Entity<TagDef>()
            //    .Property(s => s.Status)
            //    .HasDefaultValue(TagStatus.Good);

            modelBuilder.Entity<TagDef>()
                .Property(s => s.TimeStamp)
                .HasDefaultValue(DateTime.Now);

            /// Model Building for TagHist Table
            modelBuilder.Entity<TagHist>()
                .ToTable("TagHist");

            modelBuilder.Entity<TagHist>()
                .Property(s => s.TagDefId)
                .IsRequired();

            modelBuilder.Entity<TagHist>()
                .Property(s => s.TagName)
                .HasMaxLength(50)
                .IsRequired();

            //modelBuilder.Entity<TagHist>()
            //    .Property(s => s.Value)
            //    .HasDefaultValue(0.0);

            //modelBuilder.Entity<TagHist>()
            //    .Property(s => s.Status)
            //    .HasDefaultValue(TagStatus.Good);

            modelBuilder.Entity<TagHist>()
                .Property(s => s.TimeStamp)
                .HasDefaultValue(DateTime.Now);
        }
    }
}
