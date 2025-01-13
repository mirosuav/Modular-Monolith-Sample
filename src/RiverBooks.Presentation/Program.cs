using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RiverBooks.Presentation.ApiClient;
using RiverBooks.Presentation.Auth;
using RiverBooks.Presentation.Components;
using System.IdentityModel.Tokens.Jwt;

namespace RiverBooks.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Aspire and OTL
            builder.AddServiceDefaults();
            builder.Services.AddHttpContextAccessor();

            // Components
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddCascadingAuthenticationState();

            // Auth
            builder.Services.AddTransient<ApiAuthorizationHandler>();
            builder.Services.AddTransient<JwtSecurityTokenHandler>();
            builder.Services.AddScoped<JwtAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<JwtAuthenticationStateProvider>());
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
                })
                .AddHttpMessageHandler<ApiAuthorizationHandler>()
                .AddStandardResilienceHandler();


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
                JwtAuthenticationStateProvider authenticationStateProvider,
                [FromForm] string returnUrl) =>
            {
                await authenticationStateProvider.SignOut();
                return TypedResults.LocalRedirect($"~/{returnUrl}");
            });

            app.Run();
        }
    }
}