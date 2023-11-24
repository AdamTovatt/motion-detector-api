using Dapper;
using MotionDetectorApi.Helpers;
using MotionDetectorApi.Models;
using Npgsql;

namespace MotionDetectorApi.Managers
{
    public class MotionDetectorManager
    {
        public static MotionDetectorManager Instance
        {
            get
            {
                if (_instance == null) _instance = new MotionDetectorManager();
                return _instance;
            }
        }

        private static MotionDetectorManager? _instance;
        private string connectionString;

        public MotionDetectorManager()
        {
            connectionString = ConnectionStringProvider.GetConnectionString();
        }

        private async Task<NpgsqlConnection> GetConnectionAsync()
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            return connection;
        }

        public async Task<MotionDetector?> GetAsync(int id)
        {
            const string query = $"SELECT * FROM motion_detector WHERE id = @{nameof(id)}";

            using (NpgsqlConnection connection = await GetConnectionAsync())
                return (await connection.QueryAsync<MotionDetector>(query, new { id })).FirstOrDefault();
        }

        public async Task<List<MotionDetector>> GetListAsync()
        {
            const string query = "SELECT * FROM motion_detector";

            using (NpgsqlConnection connection = await GetConnectionAsync())
                return (await connection.QueryAsync<MotionDetector>(query)).ToList();
        }

        public async Task<MotionDetector> CreateNew(string name)
        {
            string secretKey = StringIdProvider.Instance.GenerateId(16);
            const string query = $"INSERT INTO motion_detector (name, secret_key) VALUES (@{nameof(name)}, @{nameof(secretKey)}) RETURNING id";

            using (NpgsqlConnection connection = await GetConnectionAsync())
                return (await GetAsync(await connection.ExecuteScalarAsync<int>(query, new { name, secretKey })))!;
        }

        public async Task<bool> RegisterMotionAsync(int id, string secretKey)
        {
            const string query = $"UPDATE motion_detector SET last_motion = NOW() WHERE id = @{nameof(id)} AND secret_key = @{nameof(secretKey)}";

            using (NpgsqlConnection connection = await GetConnectionAsync())
                return (await connection.ExecuteAsync(query, new { id, secretKey })) > 0;
        }
    }
}
