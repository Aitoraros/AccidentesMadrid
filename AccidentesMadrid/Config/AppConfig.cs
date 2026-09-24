
using Microsoft.Extensions.Configuration;

namespace AccidentesMadrid.Config;

public class AppConfig
{
    static AppConfig() {
        Config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }
    
    public static IConfiguration Config { get; }
    
}