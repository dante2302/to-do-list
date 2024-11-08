namespace API;

public static class Configuration
{
    private static readonly IConfigurationRoot _manager = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .Build();

    public static IConfigurationRoot Manager { get => _manager;  }
}