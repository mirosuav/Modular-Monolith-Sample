using Asp.Versioning;
using RiverBooks.SharedKernel.DomainEvents;
using RiverBooks.Web;
using Serilog;

var logger = Log.Logger = new LoggerConfiguration()
  .Enrich.FromLogContext()
  .WriteTo.Console()
  .CreateLogger();

logger.Information("Starting web host");

var builder = WebApplication.CreateBuilder(args);
{
    builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

    builder.Services.AddAuth(builder.Configuration);

    builder.Services.AddCommonServices();

    // Register modules
    builder.Services.AddModules(builder.Configuration, logger, out var moduleAssemblies);

    // CQRS with MediatR
    builder.Services.AddMessaging(moduleAssemblies);

    // MediatR pipeline bahaviors
    builder.Services.AddMessagingPipelineBahaviors();

    // Add MediatR Domain Event Dispatcher
    builder.Services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

    builder.Services.AddApiVersioning(opt =>
    {
        opt.DefaultApiVersion = new ApiVersion(1, 0);
        opt.AssumeDefaultVersionWhenUnspecified = true;
        opt.ReportApiVersions = true;
        opt.ApiVersionReader = new UrlSegmentApiVersionReader(); // new QueryStringApiVersionReader();
    });
}

var app = builder.Build();
{
    app.UseAuthentication()
      .UseAuthorization();



    app.MapModulesEndpoints();

    app.Run();
}

