using WeatherService.Api.Models;

namespace WeatherService.Api.Repositories;

/// <summary>
/// Persists and looks up cached weather records by coordinates.
/// </summary>
public interface IWeatherRepository
{
    Task<WeatherRecord?> GetByCoordinatesAsync(double latitude, double longitude);
    Task InsertAsync(WeatherRecord record);
}
