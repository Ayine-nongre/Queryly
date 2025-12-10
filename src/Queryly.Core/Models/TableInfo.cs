public class TableInfo
{
    public string Name {get; set;} = string.Empty;
    public string? Schema {get; set;}
    public long RowCount {get; set;} = 0;
    public long SizeInBytes {get; set;} = 0;
    public DateTime? LastModified {get; set;}
}