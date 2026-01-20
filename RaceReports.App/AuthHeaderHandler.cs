using System.Net.Http.Headers;

namespace RaceReports.App;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly AuthState _auth;

    public AuthHeaderHandler(AuthState auth) => _auth = auth;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_auth.Token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _auth.Token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
