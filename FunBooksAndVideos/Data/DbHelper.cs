using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FunBooksAndVideos.Data
{
    internal class DbHelper
    {
        public static DbErrors CheckDb(string connectionString, ILoggerFactory loggerFactory)
        {
            ILogger logger = loggerFactory.CreateLogger("DbHelper");

            logger.LogInformation("Checking database connection...");
            logger.LogDebug(nameof(connectionString) + " = " + connectionString);

            try
            {
                using var connection = new SqlConnection(connectionString);

                connection.Open();
                logger.LogTrace("Connection is open.");

                using var command = new SqlCommand("SELECT COUNT(*) FROM sys.databases WHERE name = @databaseName", connection);
                command.Parameters.AddWithValue("@databaseName", connection.Database);

                int result = (int)command.ExecuteScalar();
                logger.LogTrace("Command was executed.");

                if (result <= 0)
                {
                    logger.LogCritical("Database does not exist.");

                    return DbErrors.DatabaseDoesNotExist;
                }

                if (!CheckMigrations(connection))
                {
                    logger.LogCritical("Differences in migration.");

                    return DbErrors.DifferencesInMigration;
                }

                logger.LogInformation("Database connection check successful.");

                return DbErrors.None;
            }
            catch (ArgumentException ex)
            {
                logger.LogCritical(ex, "An error occurred while checking the database connection: " + ex.Message);

                return DbErrors.BadConnectionString;
            }
            catch (SqlException ex)
            {
                logger.LogCritical(ex, "An error occurred while checking the database connection: " + ex.Message);

                return DbErrors.General;
            }
        }

        private static string[] GetMigrationIds()
        {
            Assembly assembly = typeof(DbHelper).Assembly;
            IEnumerable<Type> types = assembly.GetTypes();

            string[] migrationIds = types.Where(type => typeof(Migration).IsAssignableFrom(type) && !type.IsAbstract)
                .Select(type => type.GetCustomAttribute<MigrationAttribute>())
                .Where(migrationAttribute => migrationAttribute != null)
                .Select(migrationAttribute => migrationAttribute.Id)
                .OrderBy(x => x)
                .ToArray();

            return migrationIds;
        }

        private static List<string> GetMigrationIds(SqlConnection connection)
        {
            const string queryString = "SELECT [MigrationId] FROM [dbo].[__EFMigrationsHistory] ORDER BY [MigrationId]";

            var migrationIds = new List<string>();

            try
            {
                using var command = new SqlCommand(queryString, connection);

                using SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string migrationId = reader.GetString(0);
                    migrationIds.Add(migrationId);
                }

                reader.Close();
            }
            catch
            {
            }

            return migrationIds;
        }

        private static bool CheckMigrations(SqlConnection connection)
        {
            string[] programMigrationIds = GetMigrationIds();
            List<string> dbMigrationIds = GetMigrationIds(connection);

            if (programMigrationIds.Length != dbMigrationIds.Count)
            {
                return false;
            }

            bool result = !programMigrationIds
                .Where((t, i) => t != dbMigrationIds[i])
                .Any();

            return result;
        }
    }
}