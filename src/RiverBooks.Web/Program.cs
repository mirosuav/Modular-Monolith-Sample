
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using RiverBooks.Books;
using RiverBooks.EmailSending.Api;
using RiverBooks.OrderProcessing.Api;
using RiverBooks.Reporting;
using RiverBooks.SharedKernel;
using RiverBooks.Users.Api;
using RiverBooks.Users.UseCases.Cart.AddItem;
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
List<Assembly> mediatRAssemblies = [typeof(Program).Assembly];
builder.Services.AddBookModuleServices(builder.Configuration, logger, mediatRAssemblies);
builder.Services.AddEmailSendingModuleServices(builder.Configuration, logger, mediatRAssemblies);
builder.Services.AddReportingModuleServices(builder.Configuration, logger, mediatRAssemblies);
builder.Services.AddOrderProcessingModuleServices(builder.Configuration, logger, mediatRAssemblies);
builder.Services.AddUserModuleServices(builder.Configuration, logger, mediatRAssemblies);

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

app.MapBookModuleEndpoints();
app.MapUserModuleEndpoints();
app.MapOrderProcessingModuleEndpoints();
app.MapEmailSendingModuleEndpoints();

app.UseOpenApi();

app.Run();

public partial class Program { } // needed for tests
