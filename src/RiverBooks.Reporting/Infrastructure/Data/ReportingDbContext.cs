﻿using System.Reflection;
using Microsoft.EntityFrameworkCore;
using RiverBooks.Reporting.Domain;

namespace RiverBooks.Reporting.Infrastructure.Data;

internal class ReportingDbContext(DbContextOptions<ReportingDbContext> options)
    : DbContext(options)
{
    public DbSet<BookSale> BookSales { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(ModuleDescriptor.Name);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(
        ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>()
            .HavePrecision(18, 6);
    }
}