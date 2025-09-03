using BankMore.AccountService.Domain.Entities;

namespace BankMore.AccountService.Domain.Interfaces;

public interface IAccountRepository
{
    Task<Account> GetByNumberAsync(string number);
    Task<Account> GetByIdAsync(int id);
    Task CreateAsync(Account account);
    Task UpdateAsync(Account account);
}