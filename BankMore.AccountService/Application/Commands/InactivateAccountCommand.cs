using MediatR;

namespace BankMore.AccountService.Application.Commands;

public class InactivateAccountCommand : IRequest
{
    public string Password { get; set; }
    public int AccountId { get; set; } 
}