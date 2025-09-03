using MediatR;

namespace BankMore.AccountService.Application.Commands;

public class LoginCommand : IRequest<string>
{
    public string Identifier { get; set; } // CPF ou número da conta
    public string Password { get; set; }
}