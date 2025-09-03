using MediatR;

namespace BankMore.AccountService.Application.Queries;

public class GetBalanceQuery : IRequest<BalanceResponse>
{
    public int AccountId { get; set; } // Do token
}

public class BalanceResponse
{
    public string AccountNumber { get; set; }
    public string Name { get; set; }
    public DateTime QueryDate { get; set; }
    public decimal Balance { get; set; }
}