using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataMasker;
using DataMasker.Interfaces;
using DataMasker.Models;
using DataMasker.Utils;

    internal class Program
    {
        private const string ConnectionString = "Server=localhost;Database=master;Trusted_Connection=True;";
        private const string ConfigFilePath = "datamasker-config.json";

        static async Task Main(string[] args)
        {
            try
            {
                // Step 1: Create and Seed the Database
                Console.WriteLine("Creating and seeding the database...");
                await CreateAndSeedDatabaseAsync();

                // Wait for the user to press a key before proceeding
                Console.WriteLine("Press any key to start data masking...");
                Console.ReadKey();

                // Step 2: Mask the Sensitive Data
                Console.WriteLine("Starting data masking...");
                await MaskSensitiveDataAsync();

                Console.WriteLine("Data masking completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates and seeds the database using a SQL script.
        /// </summary>
        private static async Task CreateAndSeedDatabaseAsync()
        {
            string sqlFilePath = Path.Combine(AppContext.BaseDirectory, "DatabaseSetup.sql");
            if (!File.Exists(sqlFilePath))
            {
                throw new FileNotFoundException($"SQL file '{sqlFilePath}' not found.");
            }

            string sqlScript = await File.ReadAllTextAsync(sqlFilePath);

            using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            foreach (string batch in sqlScript.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries))
            {
                using var command = new SqlCommand(batch, connection);
                await command.ExecuteNonQueryAsync();
            }

            Console.WriteLine("Database created and seeded successfully.");
        }

        /// <summary>
        /// Masks sensitive data based on the configuration file.
        /// </summary>
        private static async Task MaskSensitiveDataAsync()
        {
            if (!File.Exists(ConfigFilePath))
            {
                throw new FileNotFoundException($"Configuration file '{ConfigFilePath}' not found.");
            }

            // Load configuration
            Config config = Config.Load(ConfigFilePath);

            // Initialize data providers
            var dataProviders = new List<IDataProvider>
            {
                new BogusDataProvider(config.DataGeneration),
                new SqlDataProvider(new SqlConnection(config.DataSource.GetConnectionString()))
            };

            IDataMasker dataMasker = new DataMasker.DataMasker(dataProviders);
            IDataSource dataSource = DataSourceProvider.Provide(config.DataSource.Type, config.DataSource);

            // Process each table in the configuration
            foreach (TableConfig tableConfig in config.Tables)
            {
                Console.WriteLine($"Masking table: {tableConfig.Name}");

                // Load data
                IEnumerable<IDictionary<string, object>> rows = dataSource.GetData(tableConfig);

                // Mask data and update rows in the database
                var maskedRows = rows.Select(row => dataMasker.Mask(row, tableConfig));
                dataSource.UpdateRows(maskedRows, maskedRows.Count(), tableConfig);

                Console.WriteLine($"Finished masking table: {tableConfig.Name}");

            #region update row by row

            //foreach (var row in rows)
            //{
            //    //mask the data
            //    var maskedRow = dataMasker.Mask(row, tableConfig);
            //    dataSource.UpdateRow(maskedRow, tableConfig);
            //}
            #endregion

        }
    }
}
