using System.ComponentModel.DataAnnotations;

namespace RaceReports.Data.DTOs;

// Skickas in när en inloggad user vill kommentera ett inlägg
public class CommentCreateDto
{
    [Required]
    public int UserId { get; set; } // "inloggad" användare

    [Required]
    public int RaceReportId { get; set; } // vilket inlägg man kommenterar

    [Required]
    public string Text { get; set; } = string.Empty;
}
