using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using BankMore.TransferService.Domain.Entities;
using BankMore.TransferService.Domain.Interfaces;

namespace BankMore.TransferService.Infrastructure.Persistence;

public class TransferRepository : ITransferRepository
{
    private readonly string _connectionString;

    public TransferRepository(IConfiguration configuration) => _connectionString = configuration.GetConnectionString("DefaultConnection");

    public async Task CreateAsync(Transfer transfer)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(
            "INSERT INTO transferencia (idcontacorrente_origem, idcontacorrente_destino, datamovimento, valor) VALUES (@OriginAccountId, @DestinationAccountNumber, @Date, @Value)",
            transfer);
    }
}