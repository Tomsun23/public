using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Clinic.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace Clinic.Infrastructure
{
    public class ClinicDbContext : DbContext, IClinicDbContext
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PersonName> PersonNames { get; set; }

        public ClinicDbContext(DbContextOptions<ClinicDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>()
                        .HasOne(p => p.Name)
                        .WithOne(n => n.Patient)
                        .HasForeignKey<PersonName>(n => n.Id);
            base.OnModelCreating(modelBuilder);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return this.SaveChangesAsync(cancellationToken);
        }
        public Task<int> SaveChangesAsync()
        {
            return this.SaveChangesAsync();
        }
    }
}
