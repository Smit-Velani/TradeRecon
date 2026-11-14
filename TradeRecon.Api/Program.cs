using System.Threading.RateLimiting;
using TradeRecon.Api.BackgroundServices;
using TradeRecon.Api.Middleware;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TradeRecon.Application.Services;
using TradeRecon.Application.Validators;
using TradeRecon.Core.Enums;
using TradeRecon.Core.Interfaces;
using TradeRecon.Core.Models;
using TradeRecon.Infrastructure.CsvImport;
using TradeRecon.Infrastructure.Data;
using TradeRecon.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TradeReconDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("TradeRecon")
        ?? "Data Source=traderecon.db"));

builder.Services.AddScoped<ITradeRepository, EfTradeRepository>();
builder.Services.AddScoped<IAuditLogRepository, EfAuditLogRepository>();
builder.Services.AddScoped<AuditService>();
builder.Services.AddScoped<IReconciliationResultRepository, EfReconciliationResultRepository>();
builder.Services.AddScoped<IReconciliationService, ReconciliationService>();
builder.Services.AddHostedService<ReconciliationWorker>();
builder.Services.AddScoped<CsvTradeImporter>();
builder.Services.AddScoped<TradeValidator>();

builder.Services.AddHealthChecks();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode =
        StatusCodes.Status429TooManyRequests;

    options.AddPolicy(
        "api",
        context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey:
                    context.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown",
                factory: _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseRateLimiter();

app.MapControllers().RequireRateLimiting("api");
app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<TradeReconDbContext>();

    await db.Database.MigrateAsync();

    if (!await db.Trades.AnyAsync())
    {
        await db.Trades.AddRangeAsync(
            new Trade
            {
                TradeId = "T001",
                Symbol = "AAPL",
                Quantity = 100,
                Price = 278.40m,
                Currency = "USD",
                TradeDate = DateTime.UtcNow.Date,
                SourceSystem = TradeSource.Internal
            },
            new Trade
            {
                TradeId = "T002",
                Symbol = "MSFT",
                Quantity = 50,
                Price = 492.15m,
                Currency = "USD",
                TradeDate = DateTime.UtcNow.Date,
                SourceSystem = TradeSource.Internal
            },
            new Trade
            {
                TradeId = "T003",
                Symbol = "NVDA",
                Quantity = 75,
                Price = 188.20m,
                Currency = "USD",
                TradeDate = DateTime.UtcNow.Date,
                SourceSystem = TradeSource.Internal
            },
            new Trade
            {
                TradeId = "T001",
                Symbol = "AAPL",
                Quantity = 100,
                Price = 278.40m,
                Currency = "USD",
                TradeDate = DateTime.UtcNow.Date,
                SourceSystem = TradeSource.Broker
            },
            new Trade
            {
                TradeId = "T002",
                Symbol = "MSFT",
                Quantity = 55,
                Price = 492.15m,
                Currency = "USD",
                TradeDate = DateTime.UtcNow.Date,
                SourceSystem = TradeSource.Broker
            },
            new Trade
            {
                TradeId = "T004",
                Symbol = "AMZN",
                Quantity = 20,
                Price = 244.10m,
                Currency = "USD",
                TradeDate = DateTime.UtcNow.Date,
                SourceSystem = TradeSource.Broker
            }
        );

        await db.SaveChangesAsync();
    }
}

app.Run();





public partial class Program
{
}





