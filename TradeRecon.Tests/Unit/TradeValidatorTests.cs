using TradeRecon.Application.Validators;
using TradeRecon.Core.Enums;
using TradeRecon.Core.Models;

namespace TradeRecon.Tests.Unit;

public class TradeValidatorTests
{
    private readonly TradeValidator _validator = new();

    [Fact]
    public void Validate_ReturnsNoErrors_ForValidTrade()
    {
        var trade = new Trade
        {
            TradeId = "T100",
            Symbol = "AAPL",
            Quantity = 100,
            Price = 250.50m,
            Currency = "USD",
            TradeDate = DateTime.UtcNow.Date,
            SourceSystem = TradeSource.Internal
        };

        var errors = _validator.Validate(trade);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_ReturnsErrors_ForInvalidTrade()
    {
        var trade = new Trade
        {
            TradeId = "",
            Symbol = "",
            Quantity = 0,
            Price = -1,
            Currency = "",
            TradeDate = default,
            SourceSystem = TradeSource.Internal
        };

        var errors = _validator.Validate(trade);

        Assert.Equal(6, errors.Count);
    }

    [Fact]
    public void Validate_RejectsNegativeQuantity()
    {
        var trade = new Trade
        {
            TradeId = "T200",
            Symbol = "MSFT",
            Quantity = -5,
            Price = 100,
            Currency = "USD",
            TradeDate = DateTime.UtcNow.Date,
            SourceSystem = TradeSource.Broker
        };

        var errors = _validator.Validate(trade);

        Assert.Contains(
            "Quantity must be greater than zero.",
            errors);
    }
}
