using Api.Utils;
using Dapper;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Data
{
    public class DatabaseConfig : IDatabaseConfig
    {
        private string _connectionString;

        public string ConnectionString
        {
            get => _connectionString;
            set => _connectionString = value;
        }

        public DatabaseConfig(string connectionString)
        {
            DataUtils.CreateTables(connectionString);
        }

        public async Task DisposeAsync(DbConnection connection)
        {
            if (connection != null && connection.State != ConnectionState.Closed)
            {
                await connection.CloseAsync();
                await connection.DisposeAsync();
            }
            GC.SuppressFinalize(this);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State == ConnectionState.Closed) await connection.OpenAsync();
                var res = await connection.QueryAsync<T>(sql, param);
                await DisposeAsync(connection);
                return res;
            }
        }

        public async Task<int> InsertListAsync<T>(string sql, T list, object? param = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State == ConnectionState.Closed) await connection.OpenAsync();
                var trans = await connection.BeginTransactionAsync();
                var res = await connection.ExecuteAsync(sql, list, transaction: trans);
                await trans.CommitAsync();
                await DisposeAsync(connection);
                return res;
            }
        }

        public async Task<int> InsertAsync(string sql, object? param = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State == ConnectionState.Closed) await connection.OpenAsync();
                var trans = await connection.BeginTransactionAsync();
                var res = await connection.ExecuteAsync(sql, param, transaction: trans);
                await trans.CommitAsync();
                await DisposeAsync(connection);
                return res;
            }
        }

    }

}