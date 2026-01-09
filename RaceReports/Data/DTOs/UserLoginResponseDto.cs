namespace RaceReports.Data.DTOs;

// DTO = data som skickas UT vid login (krav: userId ska returneras)
public class UserLoginResponseDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
}

