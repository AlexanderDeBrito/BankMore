using BankMore.AccountService.Domain.Entities;

namespace BankMore.AccountService.Domain.Interfaces;

public interface IIdempotencyRepository
{
    Task<IdempotencyKey> GetByKeyAsync(string key);
    Task SaveAsync(IdempotencyKey idempotencyKey);
}