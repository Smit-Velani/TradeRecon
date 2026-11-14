using Microsoft.AspNetCore.Mvc;
using TradeRecon.Application.Services;
using TradeRecon.Core.Interfaces;
using TradeRecon.Core.Models;

namespace TradeRecon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReconciliationController : ControllerBase
{
    private readonly IReconciliationService _service;
    private readonly IReconciliationResultRepository _repository;
    private readonly AuditService _auditService;

    public ReconciliationController(
        IReconciliationService service,
        IReconciliationResultRepository repository,
        AuditService auditService)
    {
        _service = service;
        _repository = repository;
        _auditService = auditService;
    }

    [HttpPost("run")]
    public async Task<ActionResult<List<ReconciliationResult>>> Run()
    {
        var results = await _service.ReconcileAsync();

        await _auditService.LogAsync(
            "ReconciliationRun",
            "Reconciliation",
            $"Reconciliation completed with {results.Count} results.");

        return Ok(results);
    }

    [HttpGet]
    public async Task<ActionResult<List<ReconciliationResult>>> GetHistory()
    {
        var results = await _repository.GetAllAsync();

        return Ok(results);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReconciliationResult>> GetById(int id)
    {
        var result = await _repository.GetByIdAsync(id);

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}
