using HCareVault.Models;
using Microsoft.EntityFrameworkCore;

namespace HCareVault.Database;

public class HCareDBContext : DbContext
{
    public HCareDBContext(DbContextOptions<HCareDBContext> options) : base(options) { }

    public DbSet<FamilyMember> FamilyMembers => Set<FamilyMember>();
    public DbSet<Medication> Medications => Set<Medication>();
    public DbSet<Hospital> Hospitals => Set<Hospital>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<DoctorReferral> DoctorReferrals => Set<DoctorReferral>();
    public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
    public DbSet<RecordAttachment> RecordAttachments => Set<RecordAttachment>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Doctor>()
            .HasOne(d => d.Hospital).WithMany(h => h.Doctors)
            .HasForeignKey(d => d.HospitalId).OnDelete(DeleteBehavior.SetNull);

        b.Entity<DoctorReferral>()
            .HasOne(r => r.FromDoctor).WithMany(d => d.ReferralsMade)
            .HasForeignKey(r => r.FromDoctorId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<DoctorReferral>()
            .HasOne(r => r.ToDoctor).WithMany(d => d.ReferralsReceived)
            .HasForeignKey(r => r.ToDoctorId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<DoctorReferral>()
            .HasOne(r => r.FamilyMember).WithMany()
            .HasForeignKey(r => r.FamilyMemberId).OnDelete(DeleteBehavior.SetNull);

        b.Entity<MedicalRecord>()
            .HasOne(r => r.FamilyMember).WithMany()
            .HasForeignKey(r => r.FamilyMemberId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<MedicalRecord>()
            .HasOne(r => r.Doctor).WithMany()
            .HasForeignKey(r => r.DoctorId).OnDelete(DeleteBehavior.SetNull);
        b.Entity<MedicalRecord>()
            .HasOne(r => r.ReferredByDoctor).WithMany()
            .HasForeignKey(r => r.ReferredByDoctorId).OnDelete(DeleteBehavior.SetNull);
        b.Entity<MedicalRecord>()
            .HasOne(r => r.Hospital).WithMany()
            .HasForeignKey(r => r.HospitalId).OnDelete(DeleteBehavior.SetNull);
        b.Entity<MedicalRecord>().HasIndex(r => new { r.FamilyMemberId, r.RecordedAt });

        b.Entity<RecordAttachment>()
            .HasOne(a => a.MedicalRecord).WithMany(r => r.Attachments)
            .HasForeignKey(a => a.MedicalRecordId).OnDelete(DeleteBehavior.Cascade);
    }
}
