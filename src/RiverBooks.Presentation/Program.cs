using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RiverBooks.Presentation.ApiClient;
using RiverBooks.Presentation.Auth;
using RiverBooks.Presentation.Components;
using RiverBooks.Presentation.Extensions;

namespace RiverBooks.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Aspire and OTL
            builder.AddServiceDefaults();

            // Components
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddCircuitServicesAccessor();

            // Auth
            builder.Services.AddScoped<AuthenticationStore>();
            builder.Services.AddTransient<ApiAuthorizationHandler>();
            builder.Services.AddTransient<JwtSecurityTokenHandler>();
            builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
            builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddCookie(IdentityConstants.ApplicationScheme, o =>
                {
                    o.LoginPath = new PathString("/Account/Login");
                    o.LogoutPath = new PathString("/Account/Logout");
                });

            // API client
            builder.Services.AddHttpClient<IApiCaller, ApiCaller>(client =>
            {
                client.BaseAddress = new("https+http://riverbooks-api");
            }).AddHttpMessageHandler<ApiAuthorizationHandler>();


            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAntiforgery();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Map("/Account/Logout", async (
                AuthenticationStateProvider authenticationStateProvider,
                HttpContext context,
                [FromForm] string returnUrl) =>
            {
                await ((JwtAuthenticationStateProvider)authenticationStateProvider)
                    .SignOut(context);
                return TypedResults.LocalRedirect($"~/{returnUrl}");
            });

            app.Run();
        }
    }
}