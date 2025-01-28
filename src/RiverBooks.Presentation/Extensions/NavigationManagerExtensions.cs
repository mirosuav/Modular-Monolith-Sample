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
}