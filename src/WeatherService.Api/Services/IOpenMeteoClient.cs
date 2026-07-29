using WeatherService.Api.DTOs;

namespace WeatherService.Api.Services;

public interface IOpenMeteoClient
{
    Task<WeatherResponseDto> GetCurrentWeatherAsync(double latitude, double longitude);
}
