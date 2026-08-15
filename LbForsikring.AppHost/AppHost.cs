var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.LbForsikring>("lbforsikring");

builder.Build().Run();
