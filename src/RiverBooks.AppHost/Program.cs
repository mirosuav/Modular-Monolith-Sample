var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.RiverBooks_Web>("riverbooks-web");

builder.Build().Run();
