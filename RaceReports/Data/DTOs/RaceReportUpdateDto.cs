using System.ComponentModel.DataAnnotations;

namespace RaceReports.Data.DTOs;

public class RaceReportUpdateDto
{
    [Required]
    public int UserId { get; set; }

    [Required, MaxLength(120)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Text { get; set; } = string.Empty;

    [Required]
    public int CategoryId { get; set; }
}
