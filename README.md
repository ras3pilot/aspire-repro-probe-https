# Repro: Kubernetes publish – `WithHttpProbe` without `endpointName` mixes https scheme with http port

Minimal Aspire AppHost + one web project whose launch profile has both endpoints (`https://localhost:7018;http://localhost:5114`).

```csharp
builder.AddKubernetesEnvironment("k8s");

builder.AddProject<Projects.Web>("web")
    .WithHttpProbe(ProbeType.Readiness, "/health");
```

## Steps

Requires the .NET 10 SDK and the Aspire CLI 13.6.1.

```shell
aspire publish --apphost AppHost/AppHost.csproj -o out
```

Then look at `out/templates/web/deployment.yaml`.

## Actual

```yaml
readinessProbe:
  httpGet:
    scheme: "https"
    path: "/health"
    port: {{ .Values.parameters.web.port_http | int }}
```

The scheme comes from the `https` endpoint, the port from the `http` endpoint. The container only serves HTTP (`config.yaml`: `HTTP_PORTS`, no `HTTPS_PORTS`; `containerPort` = `port_http`), so the probe would do TLS against a plain HTTP port. In addition, the scheme is lowercase, which kube-apiserver rejects (microsoft/aspire#18271).

## Expected

Scheme and port come from the same endpoint, and for deployment targets the default is an endpoint the container actually serves (`http`), or publish fails asking for `endpointName`.

## Workaround

```csharp
.WithHttpProbe(ProbeType.Readiness, "/health", endpointName: "http")
```

## Versions

- Aspire.AppHost.Sdk 13.6.1
- Aspire.Hosting.Kubernetes 13.6.1-preview.1.26506.6
- .NET SDK 10.0.401
