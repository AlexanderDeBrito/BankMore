using BankMore.TransferService.Domain.Entities;

namespace BankMore.TransferService.Domain.Interfaces;

public interface IIdempotencyRepository
{
    Task<IdempotencyKey> GetByKeyAsync(string key);
    Task SaveAsync(IdempotencyKey idempotencyKey);
}