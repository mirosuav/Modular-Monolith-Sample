using Asp.Versioning;
using RiverBooks.Books.Api;
using RiverBooks.EmailSending.Api;
using RiverBooks.OrderProcessing.Api;
using RiverBooks.Reporting.Api;
using RiverBooks.SharedKernel.Authentication;
using RiverBooks.SharedKernel.Extensions;
using RiverBooks.SharedKernel.Messaging.PipelineBehaviors;
using RiverBooks.Users.Api;
using Serilog;
using Serilog.Settings.Configuration;
using Serilog.Sinks.OpenTelemetry;
using System.Reflection;
using RiverBooks.EventsProcessing.Api;
using RiverBooks.SharedKernel.Events;

namespace RiverBooks.Web;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton(TimeProvider.System);

        return services;
    }

    internal static IServiceCollection AddOpenApi(this IServiceCollection services)
    {

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    internal static IServiceCollection AddApiVersioning(this IServiceCollection services, ApiVersion defaultApiVersion)
    {
        services.AddApiVersioning(opt =>
        {
            opt.DefaultApiVersion = defaultApiVersion;
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.ReportApiVersions = true;
            opt.ApiVersionReader = new UrlSegmentApiVersionReader();
        });

        return services;
    }

    public static IServiceCollection AddLogging(
        this IServiceCollection services,
        ConfigurationManager configuration,
        List<Assembly> moduleAssemblies)
    {
        // Configure logging from all modules configuration
        var options = new ConfigurationReaderOptions([.. moduleAssemblies]);

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration, options)
            .Enrich.FromLogContext()
            .Enrich.With(new LogModuleNameEnricher())
            .WriteTo.Console()
            // Add Open telemetry with Sec
            //.WriteTo.OpenTelemetry(x =>
            //{
            //    x.Endpoint = "http://localhost:5341/ingest/otlp/v1/logs";
            //    x.Protocol = OtlpProtocol.HttpProtobuf;
            //    x.Headers = new Dictionary<string, string>
            //    {
            //        ["X-Seq-ApiKey"] = "gD9YQ2yDYfU4JD5uHT9H"
            //    };
            //    //x.ResourceAttributes = new Dictionary<string, object>
            //    //{
            //    //    ["module.name"] = "RiversBook.Users"
            //    //};
            //})
            .CreateLogger();

        services.AddSerilog();

        return services;
    }

    internal static IServiceCollection AddAuth(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services.AddJwtTokenBasedAuthentication(configuration);
        services.AddAuthenticatedUsersOnlyFallbackPolicy();

        services.AddScoped<IUserClaimsProvider, UserClaimsProvider>();
        services.AddScoped<IJwtTokenHandler, JwtTokenHandler>();
        return services;
    }

    internal static IServiceCollection AddModules(
        this IServiceCollection services,
        ConfigurationManager configuration,
        Serilog.ILogger logger,
        List<Assembly> moduleAssemblies)
    {
        services.AddEventsProcessingModuleServices(configuration, logger, moduleAssemblies);
        services.AddBookModuleServices(configuration, logger, moduleAssemblies);
        services.AddEmailSendingModuleServices(configuration, logger, moduleAssemblies);
        services.AddReportingModuleServices(configuration, logger, moduleAssemblies);
        services.AddOrderProcessingModuleServices(configuration, logger, moduleAssemblies);
        services.AddUserModuleServices(configuration, logger, moduleAssemblies);
        return services;
    }

    internal static IServiceCollection AddMessaging(this IServiceCollection services, List<Assembly> messagingAssemblies)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies([.. messagingAssemblies]));

        // MediatR pipeline behaviours
        services.AddLoggingBehavior();
        services.AddValidationBehavior();

        return services;
    }
}
