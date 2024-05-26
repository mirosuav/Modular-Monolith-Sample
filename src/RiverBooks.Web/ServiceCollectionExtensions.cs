using RiverBooks.Books.Api;
using RiverBooks.EmailSending.Api;
using RiverBooks.OrderProcessing.Api;
using RiverBooks.Reporting.Api;
using RiverBooks.Users.Api;
using System.Reflection;

namespace RiverBooks.Web;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddModules(
        this IServiceCollection services,
        List<Assembly> mediatRAssemblies,
        ConfigurationManager configuration,
        Serilog.ILogger logger)
    {
        services.AddBookModuleServices(configuration, logger, mediatRAssemblies);
        services.AddEmailSendingModuleServices(configuration, logger, mediatRAssemblies);
        services.AddReportingModuleServices(configuration, logger, mediatRAssemblies);
        services.AddOrderProcessingModuleServices(configuration, logger, mediatRAssemblies);
        services.AddUserModuleServices(configuration, logger, mediatRAssemblies);
        return services;
    }
}
