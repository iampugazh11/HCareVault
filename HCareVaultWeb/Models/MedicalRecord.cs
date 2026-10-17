using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCareVault.Models;

public enum RecordType
{
    [Display(Name = "Prescription")] Prescription = 0,
    [Display(Name = "Scan report")] ScanReport = 1,
    [Display(Name = "Lab report")] LabReport = 2,
    [Display(Name = "Other")] Other = 3
}

public class MedicalRecord
{
    public int Id { get; set; }

    public int FamilyMemberId { get; set; }
    public FamilyMember? FamilyMember { get; set; }

    public RecordType Type { get; set; } = RecordType.Prescription;

    [Required, StringLength(150)]
    public string Title { get; set; } = string.Empty;

    /// <summary>Date and time of the consultation / scan.</summary>
    public DateTime RecordedAt { get; set; } = DateTime.Now;

    /// <summary>Consulted doctor (prescription) or the doctor who reported / reviewed the scan.</summary>
    public int? DoctorId { get; set; }
    public Doctor? Doctor { get; set; }

    public int? HospitalId { get; set; }
    public Hospital? Hospital { get; set; }

    /// <summary>Doctor who referred the patient, or who ordered the scan / test.</summary>
    public int? ReferredByDoctorId { get; set; }
    public Doctor? ReferredByDoctor { get; set; }

    [StringLength(1000)]
    public string? Findings { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    [DataType(DataType.Date)]
    public DateTime? FollowUpOn { get; set; }

    public List<RecordAttachment> Attachments { get; set; } = new();

    public static string TypeLabel(RecordType type) => type switch
    {
        RecordType.Prescription => "Prescription",
        RecordType.ScanReport => "Scan report",
        RecordType.LabReport => "Lab report",
        _ => "Other"
    };

    /// <summary>Label for <see cref="DoctorId"/>, which depends on the record type.</summary>
    public static string DoctorLabel(RecordType type) =>
        type == RecordType.Prescription ? "Consulted doctor" : "Reported / reviewed by";

    /// <summary>Label for <see cref="ReferredByDoctorId"/>, which depends on the record type.</summary>
    public static string ReferredByLabel(RecordType type) =>
        type is RecordType.ScanReport or RecordType.LabReport ? "Ordered by" : "Referred by";
}

public class RecordAttachment
{
    public int Id { get; set; }

    public int MedicalRecordId { get; set; }
    public MedicalRecord? MedicalRecord { get; set; }

    /// <summary>Original file name shown to the user.</summary>
    [Required, StringLength(260)]
    public string FileName { get; set; } = string.Empty;

    /// <summary>Name of the file in the uploads folder.</summary>
    [Required, StringLength(100)]
    public string StoredName { get; set; } = string.Empty;

    [StringLength(100)]
    public string ContentType { get; set; } = "application/octet-stream";

    public long SizeBytes { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.Now;

    [NotMapped]
    public bool IsImage => ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
}
