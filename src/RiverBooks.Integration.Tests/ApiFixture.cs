using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
    private readonly MSSqlContainer _dbContainer;
    public const string DatabaseName = "RiverBooksTests";

    public ApiFixture()
    {
        _dbContainer = new MSSqlContainer();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
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

            // Override connection strings for each module
            configuration["ConnectionStrings:UsersConnectionString"] = dbConnection;
            configuration["ConnectionStrings:EmailSendingConnectionString"] = dbConnection;
            configuration["ConnectionStrings:ReportingConnectionString"] = dbConnection;
            configuration["ConnectionStrings:OrderProcessingConnectionString"] = dbConnection;
            configuration["ConnectionStrings:BooksConnectionString"] = dbConnection;

            config.AddConfiguration(configuration);
        });

        builder.ConfigureServices(services =>
        {
            // Replacement of
            // dotnet sql-cache create "Server=(local);Integrated Security=true;Initial Catalog=RiverBooks;Trust Server Certificate=True" OrderProcessing UserAddressesCache
            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = dbConnection;
                options.SchemaName = "OrderProcessing";
                options.TableName = "UserAddressesCache";
            });

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

    private static void MigrateDatabaseFor<TDbContext>(IServiceProvider sp) where TDbContext : DbContext
    {
        var db = sp.GetRequiredService<TDbContext>();
        db.Database.EnsureCreated();
        db.Database.Migrate();
    }

    public async Task<TDbContext> GetDbContext<TDbContext>(CancellationToken cancellationToken)
        where TDbContext : DbContext
    {
        var dbContext = Services.GetRequiredService<TDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
        return dbContext;
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}

