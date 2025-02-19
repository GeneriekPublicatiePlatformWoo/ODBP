﻿using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using ODBP.Apis.Odrc;
using ODBP.Apis.Search;
using ODBP.Config;
using ODBP.Features;
using ODBP.Features.Sitemap;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

using var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
    .WriteTo.Console(new JsonFormatter())
    .CreateLogger();

logger.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog(logger);

    // Add services to the container.

    builder.Services.AddControllers();
    builder.Services.AddHealthChecks();
    builder.Services.AddHttpClient();
    builder.Services.AddSingleton<IOdrcClientFactory, OdrcClientFactory>();
    builder.Services.AddBaseUri();

    // de sitemap duurt even om te genereren. de cache gaat in als de sitemap klaar is
    // stel een crawler draait elke dag om 01:00. dan is de sitemap bv om 01:01 klaar en dan gaat de cache in
    // als de cache dan pas de volgende dag om 01:01 verloopt, krijgt de crawler de volgende dag de gecachete waarde.
    // daarom maar een uurtje minder lang cachen
    const int DefaultCacheExpiryHours = 23;

    var cacheExpiryHours = double.TryParse(builder.Configuration["SITEMAP_CACHE_DURATION_HOURS"], out var d)
        ? d
        : DefaultCacheExpiryHours;

    builder.Services.AddOutputCache(x => x.AddPolicy(OutputCachePolicies.Sitemap,
        b => b.Expire(TimeSpan.FromHours(cacheExpiryHours))));

    builder.Services.AddSingleton<ResourcesConfig>();
    SearchClientMock.AddToServices(builder.Services, builder.Configuration);

    var app = builder.Build();

    app.UseSerilogRequestLogging(x => x.Logger = logger);
    app.UseDefaultFiles();
    app.UseOdbpStaticFiles();

    if (!app.Environment.IsDevelopment())
    {
        app.UseOutputCache();
    }

    app.UseOdbpSecurityHeaders();

    app.MapControllers();
    app.MapHealthChecks("/healthz");
    app.MapFallbackToIndexHtml();

    if (app.Environment.IsDevelopment())
    {
        await SearchClientMock.Seed(app);
    }
    
    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    logger.Write(LogEventLevel.Fatal, ex, "Application terminated unexpectedly");
}

public partial class Program { }
