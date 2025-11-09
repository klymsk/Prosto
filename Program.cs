using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prosto.Models;
using SQLitePCL;
using System.Diagnostics;

namespace Prosto;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var config = builder.Configuration;
        var dbProvider = config["Database:Provider"];

        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            switch (dbProvider)
            {
                case "SqlServer":
                    options.UseSqlServer(config.GetConnectionString("SqlServer"));
                    break;
                case "Postgres":
                    options.UseNpgsql(config.GetConnectionString("Postgres"));
                    break;
                case "Sqlite":
                    options.UseSqlite(config.GetConnectionString("Sqlite"));
                    break;
                case "InMemory":
                    options.UseInMemoryDatabase("ProstoInMemory");
                    break;
                default:
                    throw new Exception("Unknown database provider. Use 'SqlServer', 'Postgres' or 'Sqlite'.");
            }
        });

        builder.Services.AddSession();
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie()
        .AddGoogle(options =>
        {
            options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
            options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

            options.Scope.Add("profile");  // додає ім’я, аватар та інші базові дані
            options.SaveTokens = true;     // зберігає токени для подальшого використання

        });
        builder.Services.AddHttpContextAccessor();

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        //fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff
        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });

        var serviceName = "Prosto.Web";

        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing => tracing
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                .AddAspNetCoreInstrumentation(options =>
                {
                    // Додаємо додаткові поля до трейсу
                    options.EnrichWithHttpRequest = (activity, httpRequest) =>
                    {
                        activity.SetTag("client.ip", httpRequest.HttpContext.Connection.RemoteIpAddress?.ToString());
                        activity.SetTag("user.agent", httpRequest.Headers.UserAgent.ToString());
                        activity.SetTag("http.route", httpRequest.Path);
                    };

                    options.EnrichWithHttpResponse = (activity, httpResponse) =>
                    {
                        activity.SetTag("http.response_length", httpResponse.ContentLength ?? 0);
                        activity.SetTag("http.status_code", httpResponse.StatusCode);
                    };
                })
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation(o => o.SetDbStatementForText = true)
                .AddZipkinExporter(o => o.Endpoint = new Uri("http://localhost:9411/api/v2/spans"))
                .AddOtlpExporter(o => o.Endpoint = new Uri("http://localhost:4317")) // ← Jaeger приймає OTLP!
            )
            .WithMetrics(metrics => metrics
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()    // ← GC, Heap, CPU
                .AddPrometheusExporter(options => options.DisableTotalNameSuffixForCounters = true)
            );

        builder.Logging.AddOpenTelemetry(o =>
            o.AddOtlpExporter(e => e.Endpoint = new Uri("http://localhost:4317")));

        var app = builder.Build();

        // Prometheus: http://localhost:5000/metrics
        app.UseOpenTelemetryPrometheusScrapingEndpoint();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        //app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseSession();
        app.UseAuthentication();
        app.UseAuthorization();

        var activitySource = new ActivitySource("Prosto.App");
        app.MapGet("/simulate/long-job", async (ILogger<Program> logger) =>
        {
            using var mainSpan = activitySource.StartActivity("LongRunningJob", ActivityKind.Server);
            mainSpan?.SetTag("job.type", "simulation");

            for (int i = 1; i <= 5; i++)
            {
                using var step = activitySource.StartActivity($"step-{i}", ActivityKind.Internal);
                step?.SetTag("step.index", i);
                await Task.Delay(1000); // імітація роботи
            }

            logger.LogInformation("Long job finished");
            return Results.Ok(new { status = "done" });
        });

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await Prosto.Data.DbSeeder.SeedAsync(db);

        app.Run();
    }
}