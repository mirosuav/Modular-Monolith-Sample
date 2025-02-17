using Microsoft.AspNetCore.Authentication;
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
            builder.Services.AddAntiforgery();

            // Components
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();
            builder.Services.AddBlazorBootstrap();
            builder.Services.AddCascadingAuthenticationState();

            // Auth
            builder.Services.AddTransient<JwtSecurityTokenHandler>();
            builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
            builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddCookie(IdentityConstants.ApplicationScheme, o =>
                {
                    o.LoginPath = new PathString("/Account/Login");
                    o.LogoutPath = new PathString("/Account/Logout");
                });

            // API client
            builder.Services.AddHttpClient("RiverBooksApi", client =>
                {
                    client.BaseAddress = new(builder.Configuration.GetConnectionString("RiverBooksApi") 
                                             ?? throw new ApplicationException("No connection string to RiverBooksApi found!"));
                })
                .AddStandardResilienceHandler();
            builder.Services.AddScoped<IApiCaller, ApiCaller>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseAntiforgery();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode();

            app.Map("/Account/Logout", async (
                HttpContext httpContext,
                AuthenticationStateProvider authenticationStateProvider,
                [FromForm] string returnUrl) =>
            {
                await ((JwtAuthenticationStateProvider)authenticationStateProvider).SignOut();
                await httpContext.SignOutAsync();
                return TypedResults.LocalRedirect($"~/{returnUrl}");
            });



            app.Run();
        }
    }
}