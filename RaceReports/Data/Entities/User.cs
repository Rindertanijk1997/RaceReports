using System.ComponentModel.DataAnnotations;

namespace RaceReports.Data.Entities;

public class User
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;
 
    [Required]
    public string Email { get; set; } = string.Empty;

    public List<RaceReport> RaceReports { get; set; } = new();

    public List<Comment> Comments { get; set; } = new();
}
