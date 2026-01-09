using System.ComponentModel.DataAnnotations;

namespace RaceReports.Data.DTOs;

// DTO = data som skickas IN när man registrerar
public class UserRegisterDto
{
    [Required, MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(120)]
    public string Email { get; set; } = string.Empty;
}

