using System.Diagnostics;
using System.Security.Cryptography;
using DotNet.Testcontainers.Builders;
using Testcontainers.MsSql;

namespace RiverBooks.Integration.Tests;

internal class MSSqlContainer
{
    private readonly MsSqlContainer msSqlContainer;
    private readonly string sqlPassword;

    public MSSqlContainer()
    {
        try
        {
            sqlPassword = "pAs5!w0R!D" + Convert.ToBase64String(RandomNumberGenerator.GetBytes(3)).TrimEnd('=');

            msSqlContainer = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPortBinding(1433, true)
                .WithPassword(sqlPassword)
                .WithWaitStrategy(Wait
                    .ForUnixContainer()
                    .UntilCommandIsCompleted("/opt/mssql-tools18/bin/sqlcmd", "-C", "-Q", "SELECT 1;"))
                .Build();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            throw;
        }
    }

    public string GetConnectionString(string databaseName)
    {
        // return "Server=(local);Integrated Security=true;Initial Catalog=RiverBooks.Tests;Trust Server Certificate=True";
        var properties = new Dictionary<string, string>
        {
            { "Server", msSqlContainer.Hostname + "," + msSqlContainer.GetMappedPublicPort(MsSqlBuilder.MsSqlPort) },
            { "Database", databaseName ?? "master" },
            { "User Id", "sa" },
            { "Password", sqlPassword },
            { "TrustServerCertificate", "True" }
        };
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }

    public Task StartAsync()
    {
        return msSqlContainer.StartAsync();
    }

    public ValueTask DisposeAsync()
    {
        return msSqlContainer.DisposeAsync();
    }
}