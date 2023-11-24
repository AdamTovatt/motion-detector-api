﻿using MotionDetectorApi.Helpers;
using Npgsql;

namespace MotionDetectorApiTests
{
    public class DatabaseHelper
    {
        public static async Task ExecuteQueryAsync(string query)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionStringProvider.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    await command.ExecuteScalarAsync();
                }
            }
        }

        public static async Task CleanTable(string tableName)
        {
            if (tableName.Contains(" ") || tableName.Contains(";"))
                return;

            try
            {
                await ExecuteQueryAsync($"TRUNCATE {tableName} RESTART IDENTITY CASCADE");
            }
            catch (PostgresException exception)
            {
                if (exception.SqlState != "42P01")
                    throw;
            }
        }
    }
}
