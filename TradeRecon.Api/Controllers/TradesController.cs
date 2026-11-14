using Microsoft.AspNetCore.Mvc;
using TradeRecon.Core.DTOs;
using TradeRecon.Core.Enums;
using TradeRecon.Core.Interfaces;
using TradeRecon.Core.Models;

namespace TradeRecon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TradesController : ControllerBase
{
    private readonly ITradeRepository _tradeRepository;

    public TradesController(
        ITradeRepository tradeRepository)
    {
        _tradeRepository = tradeRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Trade>>> GetAll()
    {
        var trades =
            await _tradeRepository.GetAllAsync();

        return Ok(trades);
    }

    [HttpGet("search")]
    public async Task<ActionResult<PagedResult<Trade>>> Search(
        [FromQuery] string? symbol = null,
        [FromQuery] TradeSource? source = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1)
            page = 1;

        if (pageSize < 1)
            pageSize = 10;

        if (pageSize > 100)
            pageSize = 100;

        var trades =
            await _tradeRepository.GetAllAsync();

        var query = trades.AsQueryable();

        if (!string.IsNullOrWhiteSpace(symbol))
        {
            query = query.Where(
                t => t.Symbol.Equals(
                    symbol,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (source.HasValue)
        {
            query = query.Where(
                t => t.SourceSystem == source.Value);
        }

        var totalCount = query.Count();

        var items = query
            .OrderBy(t => t.TradeId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Ok(new PagedResult<Trade>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }
}
