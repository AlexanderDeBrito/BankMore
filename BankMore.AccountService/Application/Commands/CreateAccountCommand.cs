using MediatR;

namespace BankMore.AccountService.Application.Commands;

public class CreateAccountCommand : IRequest<string>
{
    public string Cpf { get; set; } 
    public string Name { get; set; }
    public string Password { get; set; }
}