using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace ImpostoreGame;

public static class NavigationManagerExtensions
{
    public static string? GetQueryParameter(this NavigationManager navigationManager, string key)
    {
        var uri = new Uri(navigationManager.Uri);
        var queryParams = QueryHelpers.ParseQuery(uri.Query);
        return queryParams.TryGetValue(key, out var value) ? value.ToString() : null;
    }
}
