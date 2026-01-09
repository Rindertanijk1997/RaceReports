using BCrypt.Net;

namespace RaceReports.Data.Services;

public static class PasswordService
{
    // Hashar lösenord med salt + work factor (default = 10)
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    // Verifierar lösenord
    public static bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
