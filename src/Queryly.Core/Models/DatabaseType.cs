public enum DatabaseType
{
    SQLite
}

public static class DatabaseTypeExtensions
{
    public static string TypeToString(this DatabaseType dbType)
    {
        return dbType switch
        {
            DatabaseType.SQLite => "SQLite",
            _ => "Unknown"
        };
    }
}