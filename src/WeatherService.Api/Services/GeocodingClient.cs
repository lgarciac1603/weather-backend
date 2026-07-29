using System.Text.Json.Serialization;

namespace WeatherService.Api.Services;

/// <summary>
/// <see cref="IGeocodingClient"/> implementation backed by the public Open-Meteo geocoding API.
/// </summary>
public class GeocodingClient : IGeocodingClient
{
    private readonly HttpClient _httpClient;

    public GeocodingClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(double Latitude, double Longitude)?> GetCoordinatesAsync(string city)
    {
        var url = $"v1/search?name={Uri.EscapeDataString(city)}&count=1&format=json";
        var response = await _httpClient.GetFromJsonAsync<GeocodingResponse>(url);

        var result = response?.Results?.FirstOrDefault();
        if (result is null)
        {
            return null;
        }

        return (result.Latitude, result.Longitude);
    }

    private record GeocodingResponse(
        [property: JsonPropertyName("results")] List<GeocodingResult>? Results);

    private record GeocodingResult(
        [property: JsonPropertyName("latitude")] double Latitude,
        [property: JsonPropertyName("longitude")] double Longitude);
}