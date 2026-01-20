using System.ComponentModel.DataAnnotations;

namespace RaceReports.Data.DTOs;

public class UserUpdateDto
{
    [Required]
    public int UserId { get; set; } 

    [Required, MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(120)]
    public string Email { get; set; } = string.Empty;

    public string? NewPassword { get; set; }
}

