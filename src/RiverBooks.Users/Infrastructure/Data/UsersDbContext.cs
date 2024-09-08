﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RiverBooks.Users.Domain;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Internal;
using RiverBooks.SharedKernel.Events;

namespace RiverBooks.Users.Infrastructure.Data;

public class UsersDbContext(DbContextOptions<UsersDbContext> options)
    : TransactionalOutboxDbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserStreetAddress> UserStreetAddresses { get; set; } = null!;

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

