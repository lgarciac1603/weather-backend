namespace WeatherService.Api.Options;

/// <summary>
/// Open-Meteo API base URLs, bound from the <c>OpenMeteoSettings</c> configuration section.
/// </summary>
public class OpenMeteoSettings
{
    public string ForecastBaseUrl { get; set; } = string.Empty;
    public string GeocodingBaseUrl { get; set; } = string.Empty;
}
