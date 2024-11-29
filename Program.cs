using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using DataMasker;
//using DataMasker.Configuration;
//using Microsoft.Extensions.Logging;

namespace DataMaskerExample
{
    class Program
    {
        private const string ConnectionString = "Server=localhost;Database=Master;Trusted_Connection=True;";

        static async Task Main(string[] args)
        {
            try
            {
                // Step 1: Create and Seed the Database
                Console.WriteLine("Creating and seeding the database...");
                await CreateAndSeedDatabaseAsync();

                // Step 2: Mask the Data
                Console.WriteLine("Starting data masking...");
                await MaskSensitiveDataAsync();

                Console.WriteLine("Data masking completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static async Task CreateAndSeedDatabaseAsync()
        {
            string sqlFilePath = Path.Combine(AppContext.BaseDirectory, "DatabaseSetup.sql");
            if (!File.Exists(sqlFilePath))
            {
                throw new FileNotFoundException($"SQL file '{sqlFilePath}' not found.");
            }

            string sqlScript = await File.ReadAllTextAsync(sqlFilePath);

            // Split the script into batches by removing `GO` and separating by `USE`
            var batches = sqlScript
                .Split(new[] { "USE " }, StringSplitOptions.RemoveEmptyEntries)
                .Select(batch => (batch.StartsWith("MaskingDemo;") ? "USE " + batch : batch).Trim());

            using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            foreach (var batch in batches)
            {
                using var command = new SqlCommand(batch, connection);
                await command.ExecuteNonQueryAsync();
            }

            Console.WriteLine("Database created and seeded successfully.");
        }

        private static async Task MaskSensitiveDataAsync()
        {
            //// Load configuration from JSON file
            //string configPath = "datamasker-config.json";
            //if (!File.Exists(configPath))
            //{
            //    throw new Exception($"Configuration file '{configPath}' not found.");
            //}

            //var config = await ConfigurationLoader.LoadConfiguration(configPath);

            //// Set up logging
            //using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            //var logger = loggerFactory.CreateLogger<Program>();

            //// Create and run the DataMasker
            //var dataMasker = new Masker(config, logger);
            //await dataMasker.RunAsync();
        }
    }
}