using BankMore.AccountService.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.AccountService.Presentation.Controllers;

[ApiController]
[Route("accounts")]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAccountCommand command)
    {
        try
        {
            var number = await _mediator.Send(command);
            return Ok(number);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message.Split(':')[1].Trim(), Type = ex.Message.Split(':')[0] });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        try
        {
            var token = await _mediator.Send(command);
            return Ok(token);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { Message = ex.Message.Split(':')[1].Trim(), Type = ex.Message.Split(':')[0] });
        }
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> Inactivate([FromBody] InactivateAccountCommand command)
    {
        try
        {
            command.AccountId = int.Parse(User.FindFirst("accountId")?.Value ?? "0");
            await _mediator.Send(command);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message.Split(':')[1].Trim(), Type = ex.Message.Split(':')[0] });
        }
        catch (UnauthorizedAccessException)
        {
            // 403 para token inválido já é handled pelo middleware
            return Forbid(); 
        }
    }
}