using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using RiverBooks.Books.Infrastructure;
using RiverBooks.EmailSending.Infrastructure;
using RiverBooks.OrderProcessing.Infrastructure.Data;
using RiverBooks.Reporting.Infrastructure.Data;
using RiverBooks.Users.Infrastructure.Data;
using RiverBooks.Web;

namespace RiverBooks.Integration.Tests;

public class ApiFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    public const string DatabaseName = "RiverBooksTests";
    private readonly MSSqlContainer _dbContainer;

    public ApiFixture()
    {
        _dbContainer = new MSSqlContainer();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var dbConnection = _dbContainer.GetConnectionString(DatabaseName);

        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Override configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            config.AddConfiguration(configuration);
        });

        builder.ConfigureServices(services =>
        {
            ReplaceDbContextRegistrationFor<UsersDbContext>(services, dbConnection);
            ReplaceDbContextRegistrationFor<ReportingDbContext>(services, dbConnection);
            ReplaceDbContextRegistrationFor<OrderProcessingDbContext>(services, dbConnection);
            ReplaceDbContextRegistrationFor<EmailSendingDbContext>(services, dbConnection);
            ReplaceDbContextRegistrationFor<BookDbContext>(services, dbConnection);

            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                MigrateDatabaseFor<UsersDbContext>(scope.ServiceProvider);
                MigrateDatabaseFor<ReportingDbContext>(scope.ServiceProvider);
                MigrateDatabaseFor<OrderProcessingDbContext>(scope.ServiceProvider);
                MigrateDatabaseFor<EmailSendingDbContext>(scope.ServiceProvider);
                MigrateDatabaseFor<BookDbContext>(scope.ServiceProvider);
            }
        });

        builder.ConfigureLogging(logging =>
        {
            // Override logging
            logging.ClearProviders();
            logging.AddConsole();
        });
    }

    private static void ReplaceDbContextRegistrationFor<TDbContext>(IServiceCollection services, string dbConnection)
        where TDbContext : DbContext
    {
        services.RemoveAll<DbContextOptions<TDbContext>>();
        services.RemoveAll<TDbContext>();
        services.AddDbContext<TDbContext>(c => c.UseSqlServer(dbConnection));
    }

    private static void MigrateDatabaseFor<TDbContext>(IServiceProvider sp) where TDbContext : DbContext
    {
        var db = sp.GetRequiredService<TDbContext>();
        db.Database.Migrate();
    }

    public TDbContext GetDbContext<TDbContext>(CancellationToken cancellationToken)
        where TDbContext : DbContext
    {
        return Services.GetRequiredService<TDbContext>();
    }
}