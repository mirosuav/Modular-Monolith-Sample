using Asp.Versioning;
using RiverBooks.Books.Api;
using RiverBooks.EmailSending.Api;
using RiverBooks.EventsProcessing.Api;
using RiverBooks.OrderProcessing.Api;
using RiverBooks.Reporting.Api;
using RiverBooks.SharedKernel.Authentication;
using RiverBooks.SharedKernel.Messaging.PipelineBehaviors;
using RiverBooks.SharedKernel.Middlewares;
using RiverBooks.Users.Api;
using System.Reflection;

namespace RiverBooks.Web;

internal static class WebHostBuilderExtensions
{
    internal static void AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSingleton(TimeProvider.System);
    }

    internal static void AddOpenApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }

    internal static void AddApiVersioning(this WebApplicationBuilder builder, ApiVersion defaultApiVersion)
    {
        builder.Services.AddApiVersioning(opt =>
        {
            opt.DefaultApiVersion = defaultApiVersion;
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.ReportApiVersions = true;
            opt.ApiVersionReader = new UrlSegmentApiVersionReader();
        })
        .AddApiExplorer();
    }

    internal static void AddAuth(this WebApplicationBuilder builder)
    {
        builder.Services.AddJwtTokenBasedAuthentication(builder.Configuration);
        builder.Services.AddAuthenticatedUsersOnlyFallbackPolicy();

        builder.Services.AddScoped<IUserClaimsProvider, UserClaimsProvider>();
        builder.Services.AddScoped<IJwtTokenHandler, JwtTokenHandler>();
    }

    private const string ModuleAssemblies = nameof(ModuleAssemblies);

    internal static List<Assembly> GetModuleAssemblies(this WebApplicationBuilder builder, bool createNew = true)
    {
        if (!builder.Host.Properties.TryGetValue(ModuleAssemblies, out var value) ||
            value is not List<Assembly> moduleAssemblies)
        {
            if (!createNew)
                throw new ApplicationException("Could not resolve module assemblies from Host.Properties!");

            moduleAssemblies = new List<Assembly>();
            builder.Host.Properties[ModuleAssemblies] = moduleAssemblies;
        }

        return moduleAssemblies;
    }

    internal static void AddModules(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        // Collect modules assemblies
        var moduleAssemblies = builder.GetModuleAssemblies();
        moduleAssemblies.Add(typeof(AppDescriptor).Assembly);

        builder.Services.AddEventsProcessingModule(configuration, moduleAssemblies);
        builder.Services.AddBooksModule(configuration, moduleAssemblies);
        builder.Services.AddEmailSendingModule(configuration, moduleAssemblies);
        builder.Services.AddReportingModule(configuration, moduleAssemblies);
        builder.Services.AddOrderProcessingModule(configuration, moduleAssemblies);
        builder.Services.AddUserModule(configuration, moduleAssemblies);
    }

    public static void MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Database verification...");
        try
        {
            Books.Api.ModuleBootstrap.MigrateDatabase(scope.ServiceProvider, logger);
            Users.Api.ModuleBootstrap.MigrateDatabase(scope.ServiceProvider, logger);
            Reporting.Api.ModuleBootstrap.MigrateDatabase(scope.ServiceProvider, logger);
            EmailSending.Api.ModuleBootstrap.MigrateDatabase(scope.ServiceProvider, logger);
            OrderProcessing.Api.ModuleBootstrap.MigrateDatabase(scope.ServiceProvider, logger);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Migrating database failed.");
            throw;
        }

        logger.LogInformation("Database up to date.");
    }

    internal static void AddMessaging(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies([.. builder.GetModuleAssemblies(false)]));

        // MediatR pipeline behaviours
        builder.Services.AddLoggingBehavior();
        builder.Services.AddValidationBehavior();
    }
}