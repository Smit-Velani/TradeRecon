using Microsoft.AspNetCore.Mvc;
using TradeRecon.Application.Services;
using TradeRecon.Application.Validators;
using TradeRecon.Core.Enums;
using TradeRecon.Core.Interfaces;
using TradeRecon.Core.Models;
using TradeRecon.Infrastructure.CsvImport;

namespace TradeRecon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImportController : ControllerBase
{
    private readonly ITradeRepository _repository;
    private readonly CsvTradeImporter _importer;
    private readonly TradeValidator _validator;
    private readonly AuditService _auditService;
    private readonly ILogger<ImportController> _logger;

    public ImportController(
        ITradeRepository repository,
        CsvTradeImporter importer,
        TradeValidator validator,
        AuditService auditService,
        ILogger<ImportController> logger)
    {
        _repository = repository;
        _importer = importer;
        _validator = validator;
        _auditService = auditService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Import(
        IFormFile file,
        TradeSource source)
    {
        if (file is null || file.Length == 0)
            return BadRequest("File is empty.");

        await using var stream = file.OpenReadStream();

        var trades =
            await _importer.ImportAsync(stream, source);

        var validTrades = new List<Trade>();
        var invalidTrades = new List<object>();
        var skippedDuplicates = new List<string>();

        foreach (var trade in trades)
        {
            var errors = _validator.Validate(trade);

            if (errors.Count > 0)
            {
                invalidTrades.Add(new
                {
                    trade.TradeId,
                    Errors = errors
                });

                continue;
            }

            var existingTrade =
                await _repository.GetByTradeIdAsync(
                    trade.TradeId,
                    source);

            if (existingTrade is not null)
            {
                skippedDuplicates.Add(trade.TradeId);
                continue;
            }

            validTrades.Add(trade);
        }

        if (validTrades.Count > 0)
        {
            await _repository.AddRangeAsync(validTrades);
        }

        await _auditService.LogAsync(
            "TradeImport",
            "Trade",
            $"Source={source}; File={file.FileName}; Imported={validTrades.Count}; Invalid={invalidTrades.Count}; Duplicates={skippedDuplicates.Count}");

        _logger.LogInformation(
            "Trade import completed. Source={Source}, Imported={Imported}, Invalid={Invalid}, Duplicates={Duplicates}",
            source,
            validTrades.Count,
            invalidTrades.Count,
            skippedDuplicates.Count);

        return Ok(new
        {
            Source = source.ToString(),
            Imported = validTrades.Count,
            Invalid = invalidTrades.Count,
            SkippedDuplicates = skippedDuplicates.Count,
            DuplicateTradeIds = skippedDuplicates,
            Errors = invalidTrades
        });
    }
}
