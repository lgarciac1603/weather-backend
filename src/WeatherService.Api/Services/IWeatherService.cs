using WeatherService.Api.DTOs;

namespace WeatherService.Api.Services;

public interface IWeatherService
{
    Task<WeatherResponseDto> GetWeatherAsync(double latitude, double longitude);
}