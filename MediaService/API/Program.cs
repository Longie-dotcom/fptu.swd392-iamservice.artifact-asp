using API.Middleware;
using Application;
using DotNetEnv;
using FSA.LaboratoryManagement.Authorization;
using Infrastructure;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Console;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// Logging
// -----------------------------
var loggerFactory = LoggerFactory.Create(logging =>
{
    logging.ClearProviders();
    logging.AddSimpleConsole(options =>
    {
        options.TimestampFormat = "[HH:mm:ss] ";
        options.ColorBehavior = LoggerColorBehavior.Enabled;
    });
    logging.SetMinimumLevel(LogLevel.Information);
});

// -----------------------------
// Core setup
// -----------------------------
builder.Services.AddGrpc();
builder.Services.AddInfrastructure(loggerFactory);
builder.Services.AddApplication();
builder.Services.AddControllers();

// -----------------------------
// Swagger
// -----------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// -----------------------------
// Kestrel
// -----------------------------
builder.WebHost.ConfigureKestrel(options =>
{
    // gRPC endpoint — requires HTTP/2
    options.ListenAnyIP(5301, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });

    // REST endpoint — uses standard HTTP/1.1
    options.ListenAnyIP(5300, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
    });
});

// -----------------------------
// Privilege Authorization
// -----------------------------
builder.Services.AddPrivilegeAuthorization();

var app = builder.Build();

// -----------------------------
// Storage Configuration
// -----------------------------
var rootPath = builder.Configuration["Storage:RootPath"]!;

if (!Directory.Exists(rootPath))
{
    Directory.CreateDirectory(rootPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(rootPath),
    RequestPath = "/images"
});

// -----------------------------
// Swagger UI
// -----------------------------
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Media Service API v1");
    c.RoutePrefix = string.Empty;
});

// -----------------------------
// Map Grpc
// -----------------------------

// -----------------------------
// Middlewares
// -----------------------------
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<SuccessResponseMiddleware>();
app.UseMiddleware<TokenClaimsMiddleware>();

app.UseRouting();
app.UseAuthorization();

// -----------------------------
// Controllers & health endpoint
// -----------------------------
app.MapControllers();
app.MapGet("/health", () => Results.Ok("Media Service running"));

// -----------------------------
// Run
// -----------------------------
app.Run();
