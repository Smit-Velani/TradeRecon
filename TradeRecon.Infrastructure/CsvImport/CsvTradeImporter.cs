using TradeRecon.Core.Enums;
using TradeRecon.Core.Models;

namespace TradeRecon.Infrastructure.CsvImport;

public class CsvTradeImporter
{
    public async Task<List<Trade>> ImportAsync(
        Stream stream,
        TradeSource source)
    {
        var trades = new List<Trade>();

        using var reader = new StreamReader(stream);

        var header = await reader.ReadLineAsync();

        if (header is null)
            return trades;

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();

            if (string.IsNullOrWhiteSpace(line))
                continue;

            var columns = line.Split(',');

            if (columns.Length < 6)
                continue;

            if (!decimal.TryParse(columns[2], out var quantity))
                continue;

            if (!decimal.TryParse(columns[3], out var price))
                continue;

            if (!DateTime.TryParse(columns[5], out var tradeDate))
                continue;

            trades.Add(new Trade
            {
                TradeId = columns[0].Trim(),
                Symbol = columns[1].Trim(),
                Quantity = quantity,
                Price = price,
                Currency = columns[4].Trim(),
                TradeDate = tradeDate,
                SourceSystem = source
            });
        }

        return trades;
    }
}
