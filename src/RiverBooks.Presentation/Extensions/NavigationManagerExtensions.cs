using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace RiverBooks.Presentation.Extensions;

internal static class NavigationManagerExtensions
{
    public const string StatusCookieName = "Identity.StatusMessage";

    private static readonly CookieBuilder StatusCookieBuilder = new()
    {
        SameSite = SameSiteMode.Strict,
        HttpOnly = true,
        IsEssential = true,
        MaxAge = TimeSpan.FromSeconds(5),
    };

    [DoesNotReturn]
    public static void RedirectTo(this NavigationManager navigationManager, string? uri)
    {
        uri ??= "";

        // Prevent open redirects.
        if (!Uri.IsWellFormedUriString(uri, UriKind.Relative))
        {
            uri = navigationManager.ToBaseRelativePath(uri);
        }

        // During static rendering, NavigateTo throws a NavigationException which is handled by the framework as a redirect.
        // So as long as this is called from a statically rendered Identity component, the InvalidOperationException is never thrown.
        navigationManager.NavigateTo(uri);
        throw new InvalidOperationException(
            $"{nameof(NavigationManagerExtensions)} can only be used during static rendering.");
    }

    [DoesNotReturn]
    public static void RedirectTo(this NavigationManager navigationManager, string uri,
        Dictionary<string, object?> queryParameters)
    {
        var uriWithoutQuery = navigationManager.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path);
        var newUri = navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);
        navigationManager.RedirectTo(newUri);
    }

    [DoesNotReturn]
    public static void RedirectToWithStatus(this NavigationManager navigationManager, string uri, string message,
        HttpContext context)
    {
        context.Response.Cookies.Append(StatusCookieName, message, StatusCookieBuilder.Build(context));
        navigationManager.RedirectTo(uri);
    }

    private static string CurrentPath(this NavigationManager navigationManager) =>
        navigationManager.ToAbsoluteUri(navigationManager.Uri).GetLeftPart(UriPartial.Path);

    [DoesNotReturn]
    public static void RedirectToCurrentPage(this NavigationManager navigationManager) =>
        navigationManager.RedirectTo(navigationManager.CurrentPath());

    [DoesNotReturn]
    public static void RedirectToCurrentPageWithStatus(this NavigationManager navigationManager, string message,
        HttpContext context)
        => navigationManager.RedirectToWithStatus(navigationManager.CurrentPath(), message, context);
}