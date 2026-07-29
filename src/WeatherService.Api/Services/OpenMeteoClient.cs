using System.Text.Json.Serialization;
using WeatherService.Api.DTOs;

namespace WeatherService.Api.Services;

/// <summary>
/// <see cref="IOpenMeteoClient"/> implementation backed by the public Open-Meteo forecast API.
/// </summary>
public class OpenMeteoClient : IOpenMeteoClient
{
    private readonly HttpClient _httpClient;

    public OpenMeteoClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherResponseDto> GetCurrentWeatherAsync(double latitude, double longitude)
    {
        var url = $"v1/forecast?latitude={latitude}&longitude={longitude}&current=temperature_2m,wind_speed_10m,wind_direction_10m&daily=sunrise&timezone=auto";

        var response = await _httpClient.GetFromJsonAsync<OpenMeteoResponse>(url)
            ?? throw new InvalidOperationException("Open-Meteo returned an empty response.");

        return new WeatherResponseDto
        {
            Temperature = response.Current.Temperature2m,
            WindDirection = response.Current.WindDirection10m,
            WindSpeed = response.Current.WindSpeed10m,
            Sunrise = DateTime.Parse(response.Daily.Sunrise[0])
        };
    }

    private record OpenMeteoResponse(
        [property: JsonPropertyName("current")] CurrentData Current,
        [property: JsonPropertyName("daily")] DailyData Daily);

    private record CurrentData(
        [property: JsonPropertyName("temperature_2m")] double Temperature2m,
        [property: JsonPropertyName("wind_speed_10m")] double WindSpeed10m,
        [property: JsonPropertyName("wind_direction_10m")] double WindDirection10m);

    private record DailyData(
        [property: JsonPropertyName("sunrise")] List<string> Sunrise);
}