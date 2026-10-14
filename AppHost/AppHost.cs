#pragma warning disable ASPIREPROBES001

var builder = DistributedApplication.CreateBuilder(args);

builder.AddKubernetesEnvironment("k8s");

// No endpointName: the probe takes the scheme from the "https" endpoint but the port from "http".
builder.AddProject<Projects.Web>("web")
    .WithHttpProbe(ProbeType.Readiness, "/health");

builder.Build().Run();
