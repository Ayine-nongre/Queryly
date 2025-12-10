public class QuerylyException: Exception
{
    public QuerylyException() {}
    public QuerylyException(string message) : base(message) {}
    public QuerylyException(string message, Exception inner) : base(message, inner) {}
    //public ConnectionException(string name): base() {String.Format("Error occurred while connecting to {0}.", name);}
}