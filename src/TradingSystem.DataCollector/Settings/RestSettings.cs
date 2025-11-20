namespace TradingSystem.DataCollector.Settings;

public class RestSettings
{
    public string RestBaseUrl { get; set; } = "https://example-data-provider/api";
    public string ApiKey { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
}