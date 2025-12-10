public static class ConfigurationHelper
{
    public static string GetConfigDirectory()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var configDir = Path.Combine(home, ".queryly");

        if (!Directory.Exists(configDir))
            Directory.CreateDirectory(configDir);
        
        return configDir;
    }

    public static string GetConnectionsFilePath()
    {
        return Path.Combine(GetConfigDirectory(), "connections.json");
    }
}