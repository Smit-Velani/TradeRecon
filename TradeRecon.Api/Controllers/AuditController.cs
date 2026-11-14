using Microsoft.AspNetCore.Mvc;
using TradeRecon.Core.Interfaces;
using TradeRecon.Core.Models;

namespace TradeRecon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditController : ControllerBase
{
    private readonly IAuditLogRepository _repository;

    public AuditController(
        IAuditLogRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuditLog>>> GetAll()
    {
        var logs =
            await _repository.GetAllAsync();

        return Ok(logs);
    }
}
