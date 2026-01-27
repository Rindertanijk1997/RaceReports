using System.ComponentModel.DataAnnotations;

namespace RaceReports.Data.Entities;

public class RaceCategory
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public List<RaceReport> RaceReports { get; set; } = new();
}
