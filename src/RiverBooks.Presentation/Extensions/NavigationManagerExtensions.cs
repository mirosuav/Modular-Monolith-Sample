using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace RiverBooks.Presentation.Extensions;

internal static class NavigationManagerExtensions
{
    public static void RedirectTo(this NavigationManager navigationManager, string? uri)
    {
        uri ??= "";

        // Prevent open redirects.
        if (!Uri.IsWellFormedUriString(uri, UriKind.Relative))
        {
            uri = navigationManager.ToBaseRelativePath(uri);
        }
        navigationManager.NavigateTo(uri);
    }

    public static string GetUriWithReturn(this NavigationManager navigationManager, string? uri, string? returnUrl = null)
    {
        uri ??= "";

        // Prevent open redirects.
        if (!Uri.IsWellFormedUriString(uri, UriKind.Relative))
        {
            uri = navigationManager.ToBaseRelativePath(uri);
        }

        return navigationManager.GetUriWithQueryParameters(uri,
            new Dictionary<string, object?>
            {
                ["ReturnUrl"] = returnUrl ?? navigationManager.Uri
            });
    }

    /// <summary>
    /// Navigate to the specified URI with a return URL.
    /// </summary>
    /// <param name="navigationManager">Navigation manager</param>
    /// <param name="uri">Target URI</param>
    /// <param name="returnUrl">ReturnUrl or null for current Uri, passed as query parameter</param>
    public static void NavigateToWithReturn(this NavigationManager navigationManager, string? uri, string? returnUrl = null)
    {
        navigationManager.NavigateTo(navigationManager.GetUriWithReturn(uri, returnUrl));
    }
}