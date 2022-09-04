using Altairis.Services.Cloudflare;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Configure to use Cloudflare proxy
app.UseCloudflare();

// Show assumed client IP
app.MapGet("/", (HttpContext context) => context.Connection?.RemoteIpAddress?.ToString());

// Run application
app.Run();
