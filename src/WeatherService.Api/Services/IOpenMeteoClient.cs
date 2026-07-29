using WeatherService.Api.DTOs;

namespace WeatherService.Api.Services;

/// <summary>
/// Fetches current weather data from the Open-Meteo forecast API.
/// </summary>
public interface IOpenMeteoClient
{
    Task<WeatherResponseDto> GetCurrentWeatherAsync(double latitude, double longitude);
}
