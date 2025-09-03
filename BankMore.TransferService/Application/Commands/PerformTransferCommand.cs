using MediatR;

namespace BankMore.TransferService.Application.Commands;

public class PerformTransferCommand : IRequest
{
    public string IdempotencyKey { get; set; }
    public string DestinationAccountNumber { get; set; }
    public decimal Value { get; set; }
    public int LoggedAccountId { get; set; } // pegar do token
}