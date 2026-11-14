using Moq;
using TradeRecon.Application.Services;
using TradeRecon.Core.Enums;
using TradeRecon.Core.Interfaces;
using TradeRecon.Core.Models;

namespace TradeRecon.Tests.Unit;

public class ReconciliationServiceTests
{
    private static Trade CreateTrade(
        string tradeId,
        decimal quantity,
        decimal price,
        TradeSource source)
    {
        return new Trade
        {
            TradeId = tradeId,
            Symbol = "TEST",
            Quantity = quantity,
            Price = price,
            Currency = "USD",
            TradeDate = DateTime.UtcNow.Date,
            SourceSystem = source
        };
    }

    [Fact]
    public async Task ReconcileAsync_ReturnsMatched_WhenTradesAreEqual()
    {
        var repository = new Mock<ITradeRepository>();

        repository
            .Setup(r => r.GetBySourceAsync(TradeSource.Internal))
            .ReturnsAsync(new List<Trade>
            {
                CreateTrade("T001", 100, 250, TradeSource.Internal)
            });

        repository
            .Setup(r => r.GetBySourceAsync(TradeSource.Broker))
            .ReturnsAsync(new List<Trade>
            {
                CreateTrade("T001", 100, 250, TradeSource.Broker)
            });

        var service = new ReconciliationService(repository.Object);

        var results = await service.ReconcileAsync();

        Assert.Single(results);
        Assert.Equal(ReconciliationStatus.Matched, results[0].Status);
    }

    [Fact]
    public async Task ReconcileAsync_DetectsQuantityMismatch()
    {
        var repository = new Mock<ITradeRepository>();

        repository
            .Setup(r => r.GetBySourceAsync(TradeSource.Internal))
            .ReturnsAsync(new List<Trade>
            {
                CreateTrade("T002", 50, 400, TradeSource.Internal)
            });

        repository
            .Setup(r => r.GetBySourceAsync(TradeSource.Broker))
            .ReturnsAsync(new List<Trade>
            {
                CreateTrade("T002", 55, 400, TradeSource.Broker)
            });

        var service = new ReconciliationService(repository.Object);

        var results = await service.ReconcileAsync();

        Assert.Equal(
            ReconciliationStatus.QuantityMismatch,
            results[0].Status);
    }

    [Fact]
    public async Task ReconcileAsync_DetectsPriceMismatch()
    {
        var repository = new Mock<ITradeRepository>();

        repository
            .Setup(r => r.GetBySourceAsync(TradeSource.Internal))
            .ReturnsAsync(new List<Trade>
            {
                CreateTrade("T003", 25, 100, TradeSource.Internal)
            });

        repository
            .Setup(r => r.GetBySourceAsync(TradeSource.Broker))
            .ReturnsAsync(new List<Trade>
            {
                CreateTrade("T003", 25, 110, TradeSource.Broker)
            });

        var service = new ReconciliationService(repository.Object);

        var results = await service.ReconcileAsync();

        Assert.Equal(
            ReconciliationStatus.PriceMismatch,
            results[0].Status);
    }

    [Fact]
    public async Task ReconcileAsync_DetectsMissingBrokerTrade()
    {
        var repository = new Mock<ITradeRepository>();

        repository
            .Setup(r => r.GetBySourceAsync(TradeSource.Internal))
            .ReturnsAsync(new List<Trade>
            {
                CreateTrade("T004", 10, 50, TradeSource.Internal)
            });

        repository
            .Setup(r => r.GetBySourceAsync(TradeSource.Broker))
            .ReturnsAsync(new List<Trade>());

        var service = new ReconciliationService(repository.Object);

        var results = await service.ReconcileAsync();

        Assert.Equal(
            ReconciliationStatus.MissingFromBroker,
            results[0].Status);
    }

    [Fact]
    public async Task ReconcileAsync_DetectsMissingInternalTrade()
    {
        var repository = new Mock<ITradeRepository>();

        repository
            .Setup(r => r.GetBySourceAsync(TradeSource.Internal))
            .ReturnsAsync(new List<Trade>());

        repository
            .Setup(r => r.GetBySourceAsync(TradeSource.Broker))
            .ReturnsAsync(new List<Trade>
            {
                CreateTrade("T005", 10, 50, TradeSource.Broker)
            });

        var service = new ReconciliationService(repository.Object);

        var results = await service.ReconcileAsync();

        Assert.Equal(
            ReconciliationStatus.MissingFromInternal,
            results[0].Status);
    }

    [Fact]
    public async Task ReconcileAsync_DetectsDuplicateTradeIds()
    {
        var repository = new Mock<ITradeRepository>();

        repository
            .Setup(r => r.GetBySourceAsync(TradeSource.Internal))
            .ReturnsAsync(new List<Trade>
            {
                CreateTrade("T999", 100, 50, TradeSource.Internal),
                CreateTrade("T999", 100, 50, TradeSource.Internal)
            });

        repository
            .Setup(r => r.GetBySourceAsync(TradeSource.Broker))
            .ReturnsAsync(new List<Trade>
            {
                CreateTrade("T999", 100, 50, TradeSource.Broker)
            });

        var service = new ReconciliationService(repository.Object);

        var results = await service.ReconcileAsync();

        Assert.Single(results);
        Assert.Equal(
            ReconciliationStatus.Duplicate,
            results[0].Status);
    }

}
