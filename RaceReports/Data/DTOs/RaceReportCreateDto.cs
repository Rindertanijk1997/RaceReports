using System.ComponentModel.DataAnnotations;

namespace RaceReports.Data.DTOs;

public class RaceReportCreateDto
{
    [Required]
    public int UserId { get; set; }   // "inloggad användare"

    [Required, MaxLength(120)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Text { get; set; } = string.Empty;

    [Required]
    public string Category { get; set; } = string.Empty;

}

