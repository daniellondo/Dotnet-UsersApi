namespace Data
{
    public interface IDatabaseConfig
    {
        string ConnectionString { get; set; }
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null);
        Task<int> InsertListAsync<T>(string sql, T list, object? param = null);
        Task<int> InsertAsync(string sql, object? param = null);

    }
}
