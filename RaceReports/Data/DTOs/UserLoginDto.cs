using System.ComponentModel.DataAnnotations;

namespace RaceReports.Data.DTOs;

// DTO = data som skickas IN när man loggar in
public class UserLoginDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
