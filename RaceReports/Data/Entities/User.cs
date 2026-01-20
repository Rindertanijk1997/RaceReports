using System.ComponentModel.DataAnnotations;

namespace RaceReports.Data.Entities;

public class User
{
    // Primärnyckel i databasen
    public int Id { get; set; }

    // Användarnamn 
    [Required, MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    // Lösenord 
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    // Email 
    [Required]
    public string Email { get; set; } = string.Empty;

    // Navigation: vilka lopp-rapporter användaren skapat
    public List<RaceReport> RaceReports { get; set; } = new();

    // Navigation: vilka kommentarer användaren skrivit
    public List<Comment> Comments { get; set; } = new();
}
