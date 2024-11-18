using Azure.Identity;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using WorkshopDemo.Core.Common;
using WorkshopDemo.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.    
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHealthChecks()
    .AddCheck<WorkshopDemoHealthCheck>(nameof(WorkshopDemoHealthCheck));
builder.Services.AddSingleton<IFileService, FileService>();
builder.Services.AddSingleton<IVersionService, VersionService>();

builder.Configuration.AddAzureKeyVault(
        new Uri($"https://kv-jlcottonma-{builder.Environment.EnvironmentName}.vault.azure.net/"),
        new DefaultAzureCredential());

// See my secrets
Console.WriteLine($"My secret value is: {builder.Configuration.GetValue<string>("SomeSecret")}");
Console.WriteLine($"My connection string value is: {builder.Configuration.GetConnectionString("MyApplication")}");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/api/healthz");

app.MapControllers();

app.Run();