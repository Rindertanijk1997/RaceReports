using System.ComponentModel.DataAnnotations;

namespace RaceReports.Data.Entities;

public class RaceCategory
{
    public int Id { get; set; }

    // Namn på kategori
    [Required]
    public string Name { get; set; } = string.Empty;

    // Alla rapporter som hör till kategorin
    public List<RaceReport> RaceReports { get; set; } = new();
}
