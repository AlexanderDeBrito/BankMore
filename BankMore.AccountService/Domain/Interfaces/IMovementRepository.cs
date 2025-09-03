using BankMore.AccountService.Domain.Entities;

namespace BankMore.AccountService.Domain.Interfaces;

public interface IMovementRepository
{
    Task CreateAsync(Movement movement);
    Task<decimal> GetSumByTypeAsync(int accountId, string type);
}