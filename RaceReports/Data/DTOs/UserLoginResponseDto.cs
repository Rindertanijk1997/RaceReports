namespace RaceReports.Data.DTOs;

public class UserLoginResponseDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = "";
    public string Token { get; set; } = "";
}

