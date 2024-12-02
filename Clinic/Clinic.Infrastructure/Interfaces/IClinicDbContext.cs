using Clinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Clinic.Infrastructure.Interfaces
{
    public interface IClinicDbContext
    {
        DbSet<Patient> Patients { get; set; }
        DbSet<PersonName> PersonNames { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
}
