namespace WeatherService.Api.Services;

/// <summary>
/// Resolves a city name to coordinates via the Open-Meteo geocoding API.
/// </summary>
public interface IGeocodingClient
{
    /// <returns>The city's coordinates, or <c>null</c> if no match was found.</returns>
    Task<(double Latitude, double Longitude)?> GetCoordinatesAsync(string city);
}