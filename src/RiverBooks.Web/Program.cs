using Asp.Versioning;

namespace RiverBooks.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        {
            builder.AddServiceDefaults();
            builder.AddModules();
            builder.AddAuth();
            builder.AddApplicationServices();
            builder.AddMessaging();
            builder.AddApiVersioning(new ApiVersion(1, 0));
            builder.AddOpenApi();
            builder.MigrateDatabase();
        }

        var app = builder.Build();
        {
            app.MapDefaultEndpoints();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseExceptionHandler();
            app.MapVersionPrompt("/").AllowAnonymous();
            app.MapLogAppRedirect("/logs").AllowAnonymous();
            app.MapModulesEndpoints();
            app.UseSwaggerDevelopmentUI();
            app.Run();
        }
    }
}