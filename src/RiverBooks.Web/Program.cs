
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;

using RiverBooks.SharedKernel;
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

builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

builder.Services.AddFastEndpoints()
  .AddJWTBearerAuth(builder.Configuration["Auth:JwtSecret"]!)
  .AddAuthorization()
  .SwaggerDocument();

// Add Module Services
List<Assembly> mediatRAssemblies = [];
builder.Services.AddModules(mediatRAssemblies, builder.Configuration, logger);

// Set up MediatR
builder.Services.AddMediatR(cfg =>
  cfg.RegisterServicesFromAssemblies(mediatRAssemblies.ToArray()));
builder.Services.AddMediatRLoggingBehavior();
builder.Services.AddMediatRFluentValidationBehavior();
builder.Services.AddValidatorsFromAssemblyContaining<AddItemToCartCommandValidator>();
// Add MediatR Domain Event Dispatcher
builder.Services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserClaimsProvider, UserClaimsProvider>();

var app = builder.Build();

app.UseAuthentication()
  .UseAuthorization();

app.MapModulesEndpoints();

app.UseOpenApi();

app.Run();

