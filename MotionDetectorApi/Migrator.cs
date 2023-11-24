using DbUp;
using DbUp.Engine;
using MotionDetectorApi.Helpers;
using System.Reflection;

namespace MotionDetectorApi
{
    public class Migrator
    {
        public static void PerformDatabaseMigrations()
        {
            string connectionString = ConnectionStringProvider.GetConnectionString();

            UpgradeEngine upgrader =
                DeployChanges.To
                    .PostgresqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), (string s) => { return s.Contains("DatabaseMigrations") && s.Split(".").Last() == "sql"; })
                    .LogToConsole()
                    .Build();

            DatabaseUpgradeResult result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                throw new Exception($"Error when performing database upgrade, failing on script: {result.ErrorScript.Name} with error {result.Error}");
            }
        }
    }
}
