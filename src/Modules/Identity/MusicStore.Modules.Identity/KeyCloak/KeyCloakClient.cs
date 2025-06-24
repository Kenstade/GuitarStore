using System.Net.Http.Json;

namespace MusicStore.Modules.Identity.KeyCloak;

internal sealed class KeyCloakClient(HttpClient httpClient)
{
    public async Task<string> RegisterUserAsync(UserRepresentation user, CancellationToken ct = default)
    {
        var responseMessage = await httpClient.PostAsJsonAsync("users", user, ct);

        responseMessage.EnsureSuccessStatusCode();
        
        return ExtractIdentityIdFromHeader(responseMessage);
    }

    private static string ExtractIdentityIdFromHeader(HttpResponseMessage responseMessage)
    {
        const string userSegmentName = "users/";
        
        var locationHeader = responseMessage.Headers.Location?.PathAndQuery;
        
        if (locationHeader is null)
        {
            throw new InvalidOperationException("No location header found.");
        }
        
        var userSegmentValueIndex = locationHeader.IndexOf(userSegmentName, StringComparison.InvariantCultureIgnoreCase);
        
        var userId = locationHeader.Substring(userSegmentValueIndex + userSegmentName.Length);
        
        return userId;
    }
}