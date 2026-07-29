using WeatherService.Api.Models;

namespace WeatherService.Api.Repositories;

public interface IWeatherRepository
{
    Task<WeatherRecord?> GetByCoordinatesAsync(double latitude, double longitude);
    Task InsertAsync(WeatherRecord record);
}
