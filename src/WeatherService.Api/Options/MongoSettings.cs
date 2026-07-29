namespace WeatherService.Api.Options;

/// <summary>
/// MongoDB connection settings, bound from the <c>MongoSettings</c> configuration section.
/// </summary>
public class MongoSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}