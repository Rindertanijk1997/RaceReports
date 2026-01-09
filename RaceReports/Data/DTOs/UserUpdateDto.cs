using System.ComponentModel.DataAnnotations;

namespace RaceReports.Data.DTOs;

// DTO = data som skickas IN när man uppdaterar konto
public class UserUpdateDto
{
    [Required]
    public int UserId { get; set; } // "inloggad userId" för att få uppdatera sig själv

    [Required, MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(120)]
    public string Email { get; set; } = string.Empty;

    // valfritt: byt lösenord (om tomt → ändra inte)
    public string? NewPassword { get; set; }
}

