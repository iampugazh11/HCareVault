using System.ComponentModel.DataAnnotations;

namespace HCareVault.Models;

public class Hospital
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Address { get; set; }

    [StringLength(30)]
    public string? Phone { get; set; }

    public List<Doctor> Doctors { get; set; } = new();
}

public class Doctor
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(80)]
    public string? Specialization { get; set; }

    public int? HospitalId { get; set; }
    public Hospital? Hospital { get; set; }

    [StringLength(30)]
    public string? Phone { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public List<DoctorReferral> ReferralsMade { get; set; } = new();
    public List<DoctorReferral> ReferralsReceived { get; set; } = new();

    public string Initials
    {
        get
        {
            var name = Name.Trim();
            if (name.StartsWith("Dr.", StringComparison.OrdinalIgnoreCase)) name = name[3..];
            else if (name.StartsWith("Dr ", StringComparison.OrdinalIgnoreCase)) name = name[3..];
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length switch
            {
                0 => "?",
                1 => char.ToUpper(parts[0][0]).ToString(),
                _ => $"{char.ToUpper(parts[0][0])}{char.ToUpper(parts[^1][0])}"
            };
        }
    }
}

/// <summary>One doctor recommending / referring a patient to another doctor.</summary>
public class DoctorReferral
{
    public int Id { get; set; }

    public int FromDoctorId { get; set; }
    public Doctor? FromDoctor { get; set; }

    public int ToDoctorId { get; set; }
    public Doctor? ToDoctor { get; set; }

    /// <summary>Optional: the family member this recommendation was made for.</summary>
    public int? FamilyMemberId { get; set; }
    public FamilyMember? FamilyMember { get; set; }

    [DataType(DataType.Date)]
    public DateTime ReferredOn { get; set; } = DateTime.Today;

    [StringLength(300)]
    public string? Reason { get; set; }
}
