var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("riverbooks-sql")
    .WithLifetime(ContainerLifetime.Session);

var db = sql.AddDatabase("riverbooks-db","RiverBooks");

builder.AddProject<Projects.RiverBooks_Web>("riverbooks-web")
    .WithReference(db)
    .WaitFor(db);

builder.Build().Run();