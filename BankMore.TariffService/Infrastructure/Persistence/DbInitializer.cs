using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System;

namespace BankMore.TariffService.Infrastructure.Persistence;

public static class DbInitializer
{
    public static void Initialize(IConfiguration configuration)
    {
        try
        {
            
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=tariff.db";
            Console.WriteLine($"Inicializando banco de dados com connection string: {connectionString}");

            // Garante que o diretório existe
            var dbPath = new FileInfo(connectionString.Replace("Data Source=", "").Replace(";", ""));
            var directory = dbPath.Directory;
            if (directory != null && !directory.Exists)
            {
                directory.Create();
                Console.WriteLine($"Diretório criado: {directory.FullName}");
            }

            
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            
            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS tarifas (
                    idtarifa INTEGER PRIMARY KEY AUTOINCREMENT,
                    idcontacorrente INTEGER NOT NULL,
                    datamovimento DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    valor DECIMAL(18,2) NOT NULL
                )";
            command.ExecuteNonQuery();
            Console.WriteLine("Tabela 'tarifas' criada ou já existe.");

            connection.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao inicializar o banco de dados: {ex.Message}");
            throw; // Re-lança a exceção para depuração
        }
    }
}