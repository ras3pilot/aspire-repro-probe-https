var app = WebApplication.CreateBuilder(args).Build();

app.MapGet("/health", () => "Healthy");

app.Run();
