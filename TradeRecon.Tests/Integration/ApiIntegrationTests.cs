using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TradeRecon.Core.Models;

namespace TradeRecon.Tests.Integration;

public class ApiIntegrationTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiIntegrationTests(
        WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTrades_ReturnsSuccessAndTrades()
    {
        var response =
            await _client.GetAsync("/api/trades");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var trades =
            await response.Content
                .ReadFromJsonAsync<List<Trade>>();

        Assert.NotNull(trades);
        Assert.NotEmpty(trades);
    }

    [Fact]
    public async Task RunReconciliation_ReturnsExpectedResults()
    {
        var response =
            await _client.PostAsync(
                "/api/reconciliation/run",
                null);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var results =
            await response.Content
                .ReadFromJsonAsync<List<ReconciliationResult>>();

        Assert.NotNull(results);
        Assert.NotEmpty(results);

        Assert.Contains(
            results,
            r => r.TradeId == "T001");
    }

    [Fact]
    public async Task SearchTrades_ReturnsPagedResult()
    {
        var response =
            await _client.GetAsync(
                "/api/trades/search?symbol=AAPL&page=1&pageSize=5");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var json =
            await response.Content.ReadAsStringAsync();

        Assert.Contains(
            "AAPL",
            json,
            StringComparison.OrdinalIgnoreCase);

        Assert.Contains(
            "totalCount",
            json,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AuditEndpoint_ReturnsAuditLogs()
    {
        await _client.PostAsync(
            "/api/reconciliation/run",
            null);

        var response =
            await _client.GetAsync("/api/audit");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var content =
            await response.Content.ReadAsStringAsync();

        Assert.Contains(
            "ReconciliationRun",
            content,
            StringComparison.OrdinalIgnoreCase);
    }
}
