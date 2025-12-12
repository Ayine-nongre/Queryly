using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace Queryly.Core.Query;

public class QueryExecutor
{
    private readonly DbConnection _connection;

    public QueryExecutor(DbConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public async Task<QueryResult> ExecuteQueryAsync(string sql)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            using var command = _connection.CreateCommand();
            command.CommandText = sql;
            
            var dataTable = new DataTable();
            using var reader = await command.ExecuteReaderAsync();
            dataTable.Load(reader);
            
            stopwatch.Stop();
            
            return QueryResult.Success(dataTable, dataTable.Rows.Count, stopwatch.Elapsed);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return QueryResult.Error(ex.Message, stopwatch.Elapsed);
        }
    }

    public async Task<int> ExecuteNonQueryAsync(string sql)
    {
        try
        {
            using var command = _connection.CreateCommand();
            command.CommandText = sql;
            return await command.ExecuteNonQueryAsync();
        }
        catch
        {
            return -1;
        }
    }

    public async Task<object?> ExecuteScalarAsync(string sql)
    {
        try
        {
            using var command = _connection.CreateCommand();
            command.CommandText = sql;
            return await command.ExecuteScalarAsync();
        }
        catch
        {
            return null;
        }
    }
}