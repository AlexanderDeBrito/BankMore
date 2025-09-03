using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using BankMore.AccountService.Domain.Entities;
using BankMore.AccountService.Domain.Interfaces;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace BankMore.AccountService.Infrastructure.Persistence
{
    public class MovementRepository : IMovementRepository, IDisposable
    {
        private readonly string _connectionString;
        private readonly ConcurrentQueue<Movement> _movementBuffer = new();
        private readonly Timer _persistTimer;

        public MovementRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");

            // WAL para reduzir locks
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            connection.Execute("PRAGMA journal_mode=WAL;");

            // Timer para o buffer a cada 5 segundos
            _persistTimer = new Timer(async _ => await PersistBufferAsync(), null, 5000, 5000);
        }

        public Task CreateAsync(Movement movement)
        {            
            _movementBuffer.Enqueue(movement);
            return Task.CompletedTask;
        }

        public async Task<decimal> GetSumByTypeAsync(int accountId, string type)
        {
            using var connection = new SqliteConnection(_connectionString);
            return await connection.ExecuteScalarAsync<decimal>(
                "SELECT COALESCE(SUM(valor), 0) FROM movimento WHERE idcontacorrente = @AccountId AND tipomovimento = @Type",
                new { AccountId = accountId, Type = type });
        }

        private async Task PersistBufferAsync()
        {
            if (_movementBuffer.IsEmpty) return;

            var movementsToPersist = new List<Movement>();
            while (_movementBuffer.TryDequeue(out var mov))
                movementsToPersist.Add(mov);

            if (movementsToPersist.Count == 0) return;

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            foreach (var movement in movementsToPersist)
            {
                await connection.ExecuteAsync(
                    "INSERT INTO movimento (idcontacorrente, datamovimento, tipomovimento, valor) " +
                    "VALUES (@AccountId, @Date, @Type, @Value)",
                    new { movement.AccountId, movement.Date, movement.Type, movement.Value },
                    transaction: transaction);
            }
            transaction.Commit();
        }

        public void Dispose()
        {
            _persistTimer?.Dispose();
            PersistBufferAsync().GetAwaiter().GetResult();
        }
    }
}
