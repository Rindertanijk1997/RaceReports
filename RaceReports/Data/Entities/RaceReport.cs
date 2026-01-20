using System.ComponentModel.DataAnnotations;

namespace RaceReports.Data.Entities;

public class RaceReport
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int CategoryId { get; set; }
    public RaceCategory? Category { get; set; }

    public List<Comment> Comments { get; set; } = new();
}
