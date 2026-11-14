using Microsoft.AspNetCore.Mvc;
using TradeRecon.Core.DTOs;
using TradeRecon.Core.Enums;
using TradeRecon.Core.Interfaces;

namespace TradeRecon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IReconciliationResultRepository _repository;

    public DashboardController(
        IReconciliationResultRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ReconciliationSummaryDto>> GetSummary()
    {
        var results = await _repository.GetAllAsync();

        var total = results.Count;

        var matched = results.Count(
            r => r.Status == ReconciliationStatus.Matched);

        var mismatched = results.Count(
            r => r.Status == ReconciliationStatus.QuantityMismatch ||
                 r.Status == ReconciliationStatus.PriceMismatch);

        var missing = results.Count(
            r => r.Status == ReconciliationStatus.MissingFromBroker ||
                 r.Status == ReconciliationStatus.MissingFromInternal);

        var duplicates = results.Count(
            r => r.Status == ReconciliationStatus.Duplicate);

        var matchRate = total == 0
            ? 0
            : Math.Round(
                (decimal)matched / total * 100,
                2);

        return Ok(new ReconciliationSummaryDto
        {
            Total = total,
            Matched = matched,
            Mismatched = mismatched,
            Missing = missing,
            Duplicates = duplicates,
            MatchRate = matchRate
        });
    }
}
