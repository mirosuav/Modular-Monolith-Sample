using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("riverbooks-sql")
    .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("riverbooksdb","RiverBooks");

var api = builder.AddProject<Projects.RiverBooks_Web>("riverbooksapi")
    .WithReference(db)
    .WaitFor(db);

builder.AddProject<Projects.RiverBooks_Presentation>("riverbooks-presentation")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();