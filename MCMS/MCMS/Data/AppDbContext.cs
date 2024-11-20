using AutoMapper.Execution;
using MCMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DeviceMS.Core.DomainLayer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.LogTo(Console.WriteLine);


        public DbSet<Measurement> Devices { get; set; } = null!;
        //public DbSet<PersonReference> PersonReferences { get; set; } = null!;

        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PersonReference>().ToTable("PersonReferences").HasKey(pr => pr.UserId);
            modelBuilder.Entity<PersonReference>()
                .Property(pr => pr.UserId)
                .ValueGeneratedNever();

            modelBuilder.Entity<Device>()
                .HasOne(d => d.User)
                .WithMany() 
                .HasForeignKey("UserId")
                .IsRequired(false);
        }*/


    }
}
