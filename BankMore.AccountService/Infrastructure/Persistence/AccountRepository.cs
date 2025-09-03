using BankMore.AccountService.Domain.Entities;
using BankMore.AccountService.Domain.Interfaces;
using Dapper;
using Microsoft.Data.Sqlite;

namespace BankMore.AccountService.Infrastructure.Persistence;

public class AccountRepository : IAccountRepository
{
    private readonly string _connectionString;

    public AccountRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<Account> GetByNumberAsync(string number)
    {
        using var connection = new SqliteConnection(_connectionString);
        var account = await connection.QueryFirstOrDefaultAsync<Account>(
            "SELECT idcontacorrente AS Id, numeiro AS Number, nome AS Name, ativo AS Active, senha AS PasswordHash, salt AS Salt " +
            "FROM contacorrente WHERE numeiro = @Number",
            new { Number = number });
        return account;
    }

    public async Task<Account> GetByIdAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        var account = await connection.QueryFirstOrDefaultAsync<Account>(
            "SELECT idcontacorrente AS Id, numeiro AS Number, nome AS Name, ativo AS Active, senha AS PasswordHash, salt AS Salt " +
            "FROM contacorrente WHERE idcontacorrente = @Id",
            new { Id = id });
        return account;
    }

    public async Task CreateAsync(Account account)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(
            "INSERT INTO contacorrente (numeiro, nome, senha, salt) VALUES (@Number, @Name, @PasswordHash, @Salt)",
            new { account.Number, account.Name, account.PasswordHash, account.Salt });
    }

    public async Task UpdateAsync(Account account)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(
            "UPDATE contacorrente SET ativo = @Active WHERE idcontacorrente = @Id",
            new { account.Active, account.Id });
    }
}