using BankMore.TransferService.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.TransferService.Presentation.Controllers;

[ApiController]
[Route("transfers")]
public class TransfersController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransfersController(IMediator mediator) => _mediator = mediator;

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Perform([FromBody] PerformTransferCommand command)
    {
        try
        {
            command.LoggedAccountId = int.Parse(User.FindFirst("accountId")?.Value ?? "0");
            await _mediator.Send(command);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message, Type = "INVALID_ACCOUNT" });
        }
    }
}