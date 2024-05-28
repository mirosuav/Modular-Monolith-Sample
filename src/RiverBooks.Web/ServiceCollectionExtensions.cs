using RiverBooks.Books.Api;
using RiverBooks.EmailSending.Api;
using RiverBooks.OrderProcessing.Api;
using RiverBooks.Reporting.Api;
using RiverBooks.SharedKernel;
using RiverBooks.SharedKernel.Authentication;
using RiverBooks.SharedKernel.Messaging.PipelineBehaviors;
using RiverBooks.Users.Api;
using System.Reflection;

namespace RiverBooks.Web;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddCommonServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton(TimeProvider.System);

        return services;
    }

    internal static IServiceCollection AddAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddJwtTokenBasedAuthentication(configuration);
        services.AddAuthenticatedUsersOnlyFallbackPolicy();

        services.AddScoped<IUserClaimsProvider, UserClaimsProvider>();
        return services;
    }

    internal static IServiceCollection AddModules(
        this IServiceCollection services,
        ConfigurationManager configuration,
        Serilog.ILogger logger,
        out List<Assembly> moduleAssemblies)
    {
        moduleAssemblies = new();
        services.AddBookModuleServices(configuration, logger, moduleAssemblies);
        services.AddEmailSendingModuleServices(configuration, logger, moduleAssemblies);
        services.AddReportingModuleServices(configuration, logger, moduleAssemblies);
        services.AddOrderProcessingModuleServices(configuration, logger, moduleAssemblies);
        services.AddUserModuleServices(configuration, logger, moduleAssemblies);
        return services;
    }

    internal static IServiceCollection AddMessaging(this IServiceCollection services, List<Assembly> messagingAssemblies)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(messagingAssemblies.ToArray()));
        return services;
    }

    internal static IServiceCollection AddMessagingPipelineBahaviors(this IServiceCollection services)
    {
        services.AddLoggingBehavior();
        services.AddValidationBehavior();
        return services;
    }
}
