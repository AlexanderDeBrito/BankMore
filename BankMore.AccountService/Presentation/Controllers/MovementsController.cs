using BankMore.AccountService.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.AccountService.Presentation.Controllers;

[ApiController]
[Route("movements")]
public class MovementsController : ControllerBase
{
    private readonly IMediator _mediator;

    public MovementsController(IMediator mediator) => _mediator = mediator;

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMovementCommand command)
    {
        try
        {
            command.LoggedAccountId = int.Parse(User.FindFirst("accountId")?.Value ?? "0");
            await _mediator.Send(command);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message.Split(':')[1].Trim(), Type = ex.Message.Split(':')[0] });
        }
    }
}