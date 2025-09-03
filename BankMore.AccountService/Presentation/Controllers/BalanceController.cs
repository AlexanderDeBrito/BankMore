using BankMore.AccountService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.AccountService.Presentation.Controllers;

[ApiController]
[Route("balance")]
public class BalanceController : ControllerBase
{
    private readonly IMediator _mediator;

    public BalanceController(IMediator mediator) => _mediator = mediator;

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var query = new GetBalanceQuery { AccountId = int.Parse(User.FindFirst("accountId")?.Value ?? "0") };
            var response = await _mediator.Send(query);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message.Split(':')[1].Trim(), Type = ex.Message.Split(':')[0] });
        }
    }
}