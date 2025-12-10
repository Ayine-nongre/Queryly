using System.Data;

public class QueryResult
{
    public DataTable? Data {get; private set;} = new DataTable();
    public int? RowsAffected {get; private set;} = 0;
    public TimeSpan ExecutionTime {get; private set;} = TimeSpan.Zero;
    public bool IsSuccess {get; private set;} = true;
    public string? ErrorMessage {get; private set;} = null;

    public static QueryResult Success(DataTable data, int rowsAffected, TimeSpan executionTime)
    {
        QueryResult result = new QueryResult{
            Data = data,
            RowsAffected = rowsAffected,
            ExecutionTime = executionTime,
            IsSuccess = true
        };
        return result;
    }
    
    public static QueryResult Error(string errorMessage, TimeSpan executionTime)
    {
        QueryResult result = new QueryResult{
            Data = new DataTable(),
            RowsAffected = 0,
            ExecutionTime = executionTime,
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
        return result;
    }
}
