using System.Data.Common;
using Microsoft.Data.Sqlite;

public class SqliteConnectionProvider: IConnectionProvider
{
    public DatabaseType Type => DatabaseType.SQLite;

    public async Task<DbConnection> OpenConnectionAsync(string connectionString)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
            }

            var connection = new SqliteConnection(connectionString);
            await connection.OpenAsync();
            return connection;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to open SQLite connection.", ex);
        }
    }

    public async Task<bool> TestConnectionAsync(string connectionString)
    {
        try
        {
            using var connection = new SqliteConnection(connectionString);
            await connection.OpenAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public List<string> GetDatabasesAsync(DbConnection connection)
    {
        try
        {
            if (connection is not SqliteConnection)
            {
                throw new ArgumentException("Connection must be a SQLite connection.", nameof(connection));
            }
            var sqlConn = (SqliteConnection)connection;
            var builder = new SqliteConnectionStringBuilder(sqlConn.ConnectionString);
            var fileName = Path.GetFileNameWithoutExtension(builder.DataSource);
            return new List<string> { fileName };
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to retrieve databases for SQLite.", ex);
        }
    }

    public async Task<List<TableInfo>> GetTablesAsync(DbConnection connection, string database)
    {
        try
        {
            if (connection is not SqliteConnection)
            {
                throw new ArgumentException("Connection must be a SQLite connection.", nameof(connection));
            }

            if (database == null)
            {
                throw new ArgumentNullException(nameof(database), "Database name cannot be null.");
            }

            var tables = new List<TableInfo>();
            var sqlConn = (SqliteConnection)connection;
            var command = sqlConn.CreateCommand();
            command.CommandText = $"SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%';";
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var tableName = reader.GetString(0);

                var countCommand = sqlConn.CreateCommand();
                countCommand.CommandText = $"SELECT COUNT(*) FROM [{tableName}];";
                var rowCount = (long)(await countCommand.ExecuteScalarAsync() ?? 0L);

                tables.Add(new TableInfo { Name = tableName, RowCount = rowCount });
            }
            return tables;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to retrieve tables for SQLite.", ex);
        }
    }

    public async Task<List<ColumnInfo>> GetColumnsAsync(DbConnection connection, string database, string table)
    {
        try
        {
            if (connection is not SqliteConnection) 
                throw new ArgumentException("Connection must be a SQLite connection.", nameof(connection));
            
            if (database == null)
                throw new ArgumentNullException(nameof(database), "Database name cannot be null.");
            
            if (table == null) 
                throw new ArgumentNullException(nameof(table), "Table name cannot be null.");
            
            var columns = new List<ColumnInfo>();
            var sqlConn = (SqliteConnection)connection;
            var command = sqlConn.CreateCommand();
            command.CommandText = $"PRAGMA table_info([{table}]);";
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var columnName = reader.GetString(1);
                var dataType = reader.GetString(2);
                var notNull = reader.GetInt32(3);
                var defaultValue = reader.IsDBNull(4) ? null : reader.GetValue(4)?.ToString();  // dflt_value
                var pk = reader.GetInt32(5);  
                
                columns.Add(new ColumnInfo 
                { 
                    Name = columnName, 
                    DataType = dataType,
                    IsNullable = notNull == 0,
                    DefaultValue = defaultValue,
                    IsPrimaryKey = pk != 0
                });
            }
            return columns;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to retrieve columns for SQLite.", ex);
        }
    }
}