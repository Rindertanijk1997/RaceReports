using System.ComponentModel.DataAnnotations;

namespace RaceReports.Data.Entities;

public class Comment
{
    public int Id { get; set; }

    [Required]
    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int RaceReportId { get; set; }
    public RaceReport? RaceReport { get; set; }


    public int UserId { get; set; }
    public User? User { get; set; }
}
