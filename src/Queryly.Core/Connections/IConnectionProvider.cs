using System.Data.Common;

public interface IConnectionProvider
{
    DatabaseType Type { get; }
    Task<DbConnection> OpenConnectionAsync(string connectionString);
    Task<bool> TestConnectionAsync(string connectionString);
    List<string> GetDatabasesAsync(DbConnection connection);
    Task<List<TableInfo>> GetTablesAsync(DbConnection connection, string database);
    Task<List<ColumnInfo>> GetColumnsAsync(DbConnection connection, string database, string table);
}