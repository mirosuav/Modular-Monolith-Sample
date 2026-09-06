using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

var builder = DistributedApplication.CreateBuilder(args);

// Database SqlServer
// Use existing local running SqlServer
var db = builder.AddConnectionString("riverbooksdb");

// Deploy and new Docker SqlServer
// var db = builder
//     .AddSqlServer("riverbooks-sql")
//     .WithLifetime(ContainerLifetime.Persistent)
//     .AddDatabase("riverbooksdb", "RiverBooks");

// Backend Web API
var api = builder
    .AddProject<Projects.RiverBooks_Web>("riverbooksapi")
    .WithReference(db)
    .WaitFor(db);

// Web Client App
builder.AddProject<Projects.RiverBooks_Presentation>("riverbooks-presentation")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
