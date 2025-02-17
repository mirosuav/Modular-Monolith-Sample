var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("riverbooks-sql")
    .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("riverbooks-db","RiverBooks");

var api = builder.AddProject<Projects.RiverBooks_Web>("riverbooks-api")
    .WithReference(db)
    .WaitFor(db);

builder.AddProject<Projects.RiverBooks_Presentation>("riverbooks-presentation")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();