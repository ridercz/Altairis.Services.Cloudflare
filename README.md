[![Altairis.Services.Cloudflare](https://img.shields.io/nuget/v/Altairis.Services.Cloudflare.svg?style=flat-square&label=NuGet)](https://www.nuget.org/packages/Altairis.Services.Cloudflare/)

# Cloudflare proxy support for ASP.NET Core

Cloudlfare is a popular service providing (among others) reverse proxy/CDN services. That means that the web server is hidden behind Cloudflare's servers and does not have direct access to client IP address.

ASP.NET Core has a solution for this: the [Forwarded Headers Middleware](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-6.0), which can read `X-Forwarded-For` and similar headers and set `HttpContext.Connection.RemoteIpAddress` property accordingly.

For these two to work together, one needs to set properly IP ranges of known networks, as published by Cloudflare. This library automates this process, by downloading list of [IPv4](https://www.cloudflare.com/ips-v4) and [IPv6](https://www.cloudflare.com/ips-v6) ranges and applying them.

## Basic usage

First, install the `Altairis.Services.Cloudflare` package.

Then, in your `Program.cs`, call the `UseCloudflare` method. This is the minimal sample application:

```csharp
using Altairis.Services.Cloudflare;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Configure to use Cloudflare proxy
app.UseCloudflare();

// Show assumed client IP
app.MapGet("/", (HttpContext context) => context.Connection?.RemoteIpAddress?.ToString());

// Run application
app.Run();
```

## Advanced usage

You may use the static method `CloudflareForwardedHeadersConfigurator.GetForwardedHeadersOptions` to get `ForwardedHeadersOptions` class configured for CloudFlare and then configure the Forwarded Headers Middleware yourself.