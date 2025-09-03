using BankMore.AccountService.Domain.Entities;
using BankMore.AccountService.Domain.Interfaces;
using Dapper;
using Microsoft.Data.Sqlite;

namespace BankMore.AccountService.Infrastructure.Persistence;

public class IdempotencyRepository : IIdempotencyRepository
{
    private readonly string _connectionString;

    public IdempotencyRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<IdempotencyKey?> GetByKeyAsync(string key)
    {
        using var connection = new SqliteConnection(_connectionString);

        var result = await connection.QueryFirstOrDefaultAsync<IdempotencyKey>(
            @"SELECT 
            chave_idempotencia AS Key, 
            requisicao AS Request, 
            resultado AS Result 
          FROM idempotencia 
          WHERE chave_idempotencia = @Key",
            new { Key = key });

        return result; // pode ser null
    }


    public async Task SaveAsync(IdempotencyKey idempotencyKey)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(
            "INSERT OR REPLACE INTO idempotencia (chave_idempotencia, requisicao, resultado) VALUES (@Key, @Request, @Result)",
            new { idempotencyKey.Key, idempotencyKey.Request, idempotencyKey.Result });
    }
}