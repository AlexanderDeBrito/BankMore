using MediatR;

namespace BankMore.AccountService.Application.Commands;

public class CreateMovementCommand : IRequest
{
    public string IdempotencyKey { get; set; }
    public string? AccountNumber { get; set; }
    public decimal Value { get; set; }
    public string Type { get; set; } // "C" or "D"
    public int LoggedAccountId { get; set; } // Vem do token

    public CreateMovementCommand(string idempotencyKey, string? accountNumber, decimal value, string type, int loggedAccountId)
    {
        IdempotencyKey = idempotencyKey;
        AccountNumber = accountNumber;
        Value = value;
        Type = type;
        LoggedAccountId = loggedAccountId;
    }
}