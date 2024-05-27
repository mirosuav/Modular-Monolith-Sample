using RiverBooks.SharedKernel;
using RiverBooks.SharedKernel.DomainEvents;
using RiverBooks.SharedKernel.Messaging.PipelineBehaviors;
using RiverBooks.Users.UseCases.Cart.AddItem;
using RiverBooks.Web;
using Serilog;
using System.Reflection;

var logger = Log.Logger = new LoggerConfiguration()
  .Enrich.FromLogContext()
  .WriteTo.Console()
  .CreateLogger();

logger.Information("Starting web host");

var builder = WebApplication.CreateBuilder(args);
{
    builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

    builder.Services.AddAuth();

    builder.Services.AddCommonServices();

    // Register modules
    builder.Services.AddModules(builder.Configuration, logger, out var moduleAssemblies);

    // CQRS with MediatR
    builder.Services.AddMessaging(moduleAssemblies);

    // MediatR pipeline bahaviors
    builder.Services.AddMessagingPipelineBahaviors();

    // Add MediatR Domain Event Dispatcher
    builder.Services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();
}

var app = builder.Build();
{
    app.UseAuthentication()
      .UseAuthorization();

    app.MapModulesEndpoints();

    app.Run();
}

