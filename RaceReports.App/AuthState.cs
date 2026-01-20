namespace RaceReports.App;

public class AuthState
{
    public string? Token { get; private set; }
    public int? UserId { get; private set; }

    public bool IsLoggedIn => !string.IsNullOrWhiteSpace(Token);

    public event Action? OnChange;

    public void SetAuth(string? token, int? userId)
    {
        Token = string.IsNullOrWhiteSpace(token) ? null : token;
        UserId = userId;
        OnChange?.Invoke();
    }

    public void Logout()
    {
        Token = null;
        UserId = null;
        OnChange?.Invoke();
    }
}
