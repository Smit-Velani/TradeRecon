using TradeRecon.Core.Models;

namespace TradeRecon.Application.Validators;

public class TradeValidator
{
    public List<string> Validate(Trade trade)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(trade.TradeId))
            errors.Add("TradeId is required.");

        if (string.IsNullOrWhiteSpace(trade.Symbol))
            errors.Add("Symbol is required.");

        if (trade.Quantity <= 0)
            errors.Add("Quantity must be greater than zero.");

        if (trade.Price <= 0)
            errors.Add("Price must be greater than zero.");

        if (string.IsNullOrWhiteSpace(trade.Currency))
            errors.Add("Currency is required.");

        if (trade.TradeDate == default)
            errors.Add("TradeDate is required.");

        return errors;
    }
}
