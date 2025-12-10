using System.Text.Json;

namespace Queryly.Core.Connections;

public class ConnectionManager
{
    private readonly string _filePath;
    private string? _activeConnectionId;
    private List<ConnectionInfo> _connections;
    private bool _isLoaded;

    public ConnectionManager(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        
        _filePath = filePath;
        _connections = new List<ConnectionInfo>();
        _isLoaded = false;
    }

    private async Task EnsureLoadedAsync()
    {
        if (_isLoaded) return;
        
        if (!File.Exists(_filePath))
        {
            _connections = new List<ConnectionInfo>();
            _isLoaded = true;
            return;
        }

        try
        {
            var json = await File.ReadAllTextAsync(_filePath);
            var data = JsonSerializer.Deserialize<ConnectionsFile>(json);
            _connections = data?.Connections ?? new List<ConnectionInfo>();
            _activeConnectionId = data?.ActiveConnectionId;
            _isLoaded = true;
        }
        catch (JsonException ex)
        {
            throw new Exception("Failed to load connections file. The file may be corrupted.", ex);
        }
    }

    private async Task SaveConnectionsAsync()
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var data = new ConnectionsFile
        {
            Connections = _connections,
            ActiveConnectionId = _activeConnectionId
        };

        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_filePath, json);
    }

    public async Task<List<ConnectionInfo>> GetAllAsync()
    {
        await EnsureLoadedAsync();
        return _connections;
    }

    public async Task<ConnectionInfo?> GetByNameAsync(string name)
    {
        await EnsureLoadedAsync();
        return _connections.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<ConnectionInfo?> GetByIdAsync(string id)
    {
        await EnsureLoadedAsync();
        return _connections.FirstOrDefault(c => c.Id == id);
    }

    public async Task SaveAsync(ConnectionInfo connection)
{
    await EnsureLoadedAsync();
    
    // Check if we're updating an existing connection (by ID)
    var existingById = _connections.FirstOrDefault(c => c.Id == connection.Id);
    
    // Check if name is already used by a DIFFERENT connection
    var existingByName = _connections.FirstOrDefault(c => 
        c.Name.Equals(connection.Name, StringComparison.OrdinalIgnoreCase) 
        && c.Id != connection.Id);  // ← Important!
    
    if (existingByName != null)
    {
        throw new ConfigurationException($"A connection with the name '{connection.Name}' already exists.");
    }
    
    if (existingById != null)
    {
        _connections.Remove(existingById);
    }
    
    connection.UpdateLastUsed();
    _connections.Add(connection);
    
    await SaveConnectionsAsync();
}

    public async Task DeleteAsync(string id)
    {
        await EnsureLoadedAsync();
        
        var existing = _connections.FirstOrDefault(c => c.Id == id);
        if (existing != null)
        {
            _connections.Remove(existing);
            
            if (_activeConnectionId == id)
            {
                _activeConnectionId = null;
            }
            
            await SaveConnectionsAsync();
        }
    }

    public async Task SetActiveConnectionAsync(string id)
    {
        await EnsureLoadedAsync();
        
        if (!_connections.Any(c => c.Id == id))
        {
            throw new Exception($"Connection with ID '{id}' not found.");
        }
        
        _activeConnectionId = id;
        await SaveConnectionsAsync();
    }

    public async Task<ConnectionInfo?> GetActiveConnectionAsync()
    {
        await EnsureLoadedAsync();
        
        if (string.IsNullOrWhiteSpace(_activeConnectionId))
            return null;

        return _connections.FirstOrDefault(c => c.Id == _activeConnectionId);
    }

    private class ConnectionsFile
    {
        public List<ConnectionInfo> Connections { get; set; } = new();
        public string? ActiveConnectionId { get; set; }
    }
}