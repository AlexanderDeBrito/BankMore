using Dapper;
using Microsoft.Data.Sqlite;

namespace BankMore.AccountService.Infrastructure.Persistence;

public static class DbInitializer
{
    public static void Initialize(IConfiguration configuration)
    {
        using var connection = new SqliteConnection(configuration.GetConnectionString("DefaultConnection"));
        connection.Open();

        // Criar tabela contacorrente
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS contacorrente (
                idcontacorrente INTEGER PRIMARY KEY AUTOINCREMENT,
                numeiro TEXT NOT NULL UNIQUE, -- CPF como número da conta, único
                nome TEXT NOT NULL,
                ativo INTEGER NOT NULL DEFAULT 1,
                senha TEXT NOT NULL,
                salt TEXT NOT NULL
            )");

        // Criar tabela movimento
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS movimento (
                idmovimento INTEGER PRIMARY KEY AUTOINCREMENT,
                idcontacorrente INTEGER NOT NULL,
                datamovimento DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                tipomovimento TEXT NOT NULL CHECK(tipomovimento IN ('C', 'D')),
                valor DECIMAL(18,2) NOT NULL,
                FOREIGN KEY (idcontacorrente) REFERENCES contacorrente(idcontacorrente)
            )");

        // Criar tabela idempotencia
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS idempotencia (
                chave_idempotencia TEXT PRIMARY KEY,
                requisicao TEXT NOT NULL,
                resultado TEXT NOT NULL
            )");
    }
}