using System.ComponentModel.DataAnnotations;

namespace HCareVault.Models;

public enum MealTiming
{
    [Display(Name = "Not taken")] None = 0,
    [Display(Name = "Before food")] BeforeFood = 1,
    [Display(Name = "After food")] AfterFood = 2
}

public enum MedicationStatus
{
    Active = 0,
    Stopped = 1
}

public class Medication
{
    public int Id { get; set; }

    public int FamilyMemberId { get; set; }
    public FamilyMember? FamilyMember { get; set; }

    [Required, StringLength(120), Display(Name = "Medicine name")]
    public string MedicineName { get; set; } = string.Empty;

    [StringLength(120), Display(Name = "Consultant doctor")]
    public string? ConsultantDoctor { get; set; }

    /// <summary>Cumulative tablets purchased across all purchases.</summary>
    [Range(0, 100000), Display(Name = "Tablets bought")]
    public int TabletsBought { get; set; }

    [Range(0, 100000), Display(Name = "Tablets left")]
    public int TabletsLeft { get; set; }

    [DataType(DataType.Date), Display(Name = "Last purchased on")]
    public DateTime? LastPurchasedOn { get; set; }

    [Display(Name = "Morning")]   public MealTiming Morning { get; set; }
    [Display(Name = "Afternoon")] public MealTiming Afternoon { get; set; }
    [Display(Name = "Night")]     public MealTiming Night { get; set; }

    public MedicationStatus Status { get; set; } = MedicationStatus.Active;

    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>Optional photo of the medicine strip / box (file name in the uploads folder).</summary>
    [StringLength(100)]
    public string? ImageFile { get; set; }
}
