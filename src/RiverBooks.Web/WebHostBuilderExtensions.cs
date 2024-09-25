using Asp.Versioning;
using RiverBooks.Books.Api;
using RiverBooks.EmailSending.Api;
using RiverBooks.EventsProcessing.Api;
using RiverBooks.OrderProcessing.Api;
using RiverBooks.Reporting.Api;
using RiverBooks.SharedKernel.Authentication;
using RiverBooks.SharedKernel.Extensions;
using RiverBooks.SharedKernel.Messaging.PipelineBehaviors;
using RiverBooks.SharedKernel.Middlewares;
using RiverBooks.Users.Api;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
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
        });
    }
    public static void AddLogging(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .UseCommonSerilogConfiguration()
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
            .WriteToSeq(builder.Configuration)
            .CreateLogger();

        builder.Services.AddSerilog();
    }

    public static LoggerConfiguration UseCommonSerilogConfiguration(this LoggerConfiguration configuration)
    {
        configuration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Error)
            .Enrich.FromLogContext()
            .Enrich.With(new ModuleNameEnricher())
            .WriteTo.Console(
                theme: AnsiConsoleTheme.Literate,
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {ModuleName} {Message:lj} {NewLine}{Exception}"
            // outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] ({ModuleName}) {Message:lj} <s:{SourceContext}>{NewLine}{Exception}"
            );
        return configuration;
    }

    public static LoggerConfiguration WriteToSeq(this LoggerConfiguration loggerConfiguration, IConfiguration configuration)
    {
        var seqIngestionUrl = configuration["LogMonitoringIngestionUrl"];

        if (seqIngestionUrl is not null)
            loggerConfiguration.WriteTo.Seq(seqIngestionUrl);

        return loggerConfiguration;
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
        var logger = Log.Logger;

        // Collect modules assemblies
        var moduleAssemblies = builder.GetModuleAssemblies();
        moduleAssemblies.Add(typeof(AppDescriptor).Assembly);

        builder.Services.AddEventsProcessingModule(configuration, logger, moduleAssemblies);
        builder.Services.AddBooksModule(configuration, logger, moduleAssemblies);
        builder.Services.AddEmailSendingModule(configuration, logger, moduleAssemblies);
        builder.Services.AddReportingModule(configuration, logger, moduleAssemblies);
        builder.Services.AddOrderProcessingModule(configuration, logger, moduleAssemblies);
        builder.Services.AddUserModule(configuration, logger, moduleAssemblies);
    }

    internal static void AddMessaging(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies([.. builder.GetModuleAssemblies(false)]));

        // MediatR pipeline behaviours
        builder.Services.AddLoggingBehavior();
        builder.Services.AddValidationBehavior();
    }
}