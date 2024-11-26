using AutoMapper.Execution;
using MCMS.Models;
using MCMS.Models.DTOModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text.Json;


namespace MCMS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.LogTo(Console.WriteLine);

        public DbSet<DeviceConsumption> DeviceConsumptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DeviceConsumption>(entity =>
            {
                entity.HasKey(e => e.DeviceId);
                entity.Property(e => e.HourlyConsumption)
                      .HasConversion(
                          v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                          v => JsonSerializer.Deserialize<Dictionary<long, float>>(v, (JsonSerializerOptions?)null) 
                      );
            });
        }



    }
}
