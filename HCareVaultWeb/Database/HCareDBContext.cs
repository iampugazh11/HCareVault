using HCareVault.Models;
using Microsoft.EntityFrameworkCore;

namespace HCareVault.Database;

public class HCareDBContext : DbContext
{
    public HCareDBContext(DbContextOptions<HCareDBContext> options) : base(options) { }

    public DbSet<FamilyMember> FamilyMembers => Set<FamilyMember>();
    public DbSet<Medication> Medications => Set<Medication>();
}
