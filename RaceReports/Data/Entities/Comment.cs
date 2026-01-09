using System.ComponentModel.DataAnnotations;

namespace RaceReports.Data.Entities;

public class Comment
{
    public int Id { get; set; }

    // Kommentartext
    [Required]
    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // FK → RaceReport
    public int RaceReportId { get; set; }
    public RaceReport? RaceReport { get; set; }

    // FK → User (den som kommenterar)
    public int UserId { get; set; }
    public User? User { get; set; }
}
