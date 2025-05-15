using Microsoft.EntityFrameworkCore;
using PhoenixContact.EF.Model;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace PhoenixContact.EF
{
    public class PhoenixContactDb : DbContext
    {
        public PhoenixContactDb(DbContextOptions<PhoenixContactDb> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PositionLevel).HasMaxLength(50);
                entity.Property(e => e.Residence).HasMaxLength(100);
            });
        }
    }
}
