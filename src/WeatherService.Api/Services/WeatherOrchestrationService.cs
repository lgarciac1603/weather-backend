using WeatherService.Api.DTOs;
using WeatherService.Api.Models;
using WeatherService.Api.Repositories;

namespace WeatherService.Api.Services;

/// <summary>
/// Orchestrates the cache-first weather lookup: MongoDB is checked before falling back
/// to Open-Meteo, and any new result is persisted so the next call with the same
/// coordinates never hits the external API again.
/// </summary>
public class WeatherOrchestrationService : IWeatherService
{
    private readonly IWeatherRepository _repository;
    private readonly IOpenMeteoClient _openMeteoClient;

    public WeatherOrchestrationService(IWeatherRepository repository, IOpenMeteoClient openMeteoClient)
    {
        _repository = repository;
        _openMeteoClient = openMeteoClient;
    }

    public async Task<WeatherResponseDto> GetWeatherAsync(double latitude, double longitude)
    {
        var cached = await _repository.GetByCoordinatesAsync(latitude, longitude);
        if (cached is not null)
        {
            return new WeatherResponseDto
            {
                Temperature = cached.Temperature,
                WindDirection = cached.WindDirection,
                WindSpeed = cached.WindSpeed,
                Sunrise = cached.Sunrise
            };
        }

        var weather = await _openMeteoClient.GetCurrentWeatherAsync(latitude, longitude);

        await _repository.InsertAsync(new WeatherRecord
        {
            Latitude = latitude,
            Longitude = longitude,
            Temperature = weather.Temperature,
            WindDirection = weather.WindDirection,
            WindSpeed = weather.WindSpeed,
            Sunrise = weather.Sunrise,
            FetchedAt = DateTime.UtcNow
        });

        return weather;
    }
}
