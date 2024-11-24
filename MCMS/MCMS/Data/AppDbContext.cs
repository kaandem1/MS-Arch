using AutoMapper.Execution;
using MCMS.Models;
using MCMS.Models.DTOModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MCMS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.LogTo(Console.WriteLine);


        public DbSet<DeviceInfoDTO> Devices { get; set; }
        public DbSet<Measurement> Measurements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<DeviceInfoDTO>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.MaxHourlyCons).IsRequired();
            });

            modelBuilder.Entity<Measurement>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Timestamp).IsRequired();
                entity.Property(m => m.MeasurementValue).IsRequired();

                entity.HasOne(m => m.Device)
                      .WithMany(d => d.Measurements)
                      .HasForeignKey(m => m.DeviceId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }


    }
}
