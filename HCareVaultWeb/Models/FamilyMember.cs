using System.ComponentModel.DataAnnotations;

namespace HCareVault.Models;

public class FamilyMember
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [StringLength(40)]
    public string? Relationship { get; set; }

    public List<Medication> Medications { get; set; } = new();
}
