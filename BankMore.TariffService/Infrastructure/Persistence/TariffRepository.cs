using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using BankMore.TariffService.Domain.Entities;

namespace BankMore.TariffService.Infrastructure.Persistence;

public class TariffRepository
{
    private readonly string _connectionString;

    public TariffRepository(IConfiguration configuration) => _connectionString = configuration.GetConnectionString("DefaultConnection");

    public async Task CreateAsync(Tariff tariff)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(
            "INSERT INTO tarifas (idcontacorrente, valor, datamovimento) VALUES (@AccountId, @Value, @Date)",
            tariff);
    }
}