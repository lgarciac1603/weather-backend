using WeatherService.Api.DTOs;

namespace WeatherService.Api.Services;

/// <summary>
/// Resolves weather data for a location, using MongoDB as a cache in front of Open-Meteo.
/// </summary>
public interface IWeatherService
{
    Task<WeatherResponseDto> GetWeatherAsync(double latitude, double longitude);
}