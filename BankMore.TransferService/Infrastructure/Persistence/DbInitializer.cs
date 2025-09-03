using Dapper;
using Microsoft.Data.Sqlite;

namespace BankMore.TransferService.Infrastructure.Persistence;

public static class DbInitializer
{
    public static void Initialize(IConfiguration configuration)
    {
        using var connection = new SqliteConnection(configuration.GetConnectionString("DefaultConnection"));
        connection.Open();

        // Tabela transferencia
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS transferencia (
                idtransferencia INTEGER PRIMARY KEY AUTOINCREMENT,
                idcontacorrente_origem INTEGER NOT NULL,
                idcontacorrente_destino TEXT NOT NULL, -- Como string, pois destino é number
                datamovimento DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                valor DECIMAL(18,2) NOT NULL
            )");

        // Tabela idempotencia
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS idempotencia (
                chave_idempotencia TEXT PRIMARY KEY,
                requisicao TEXT NOT NULL,
                resultado TEXT NOT NULL
            )");
    }
}