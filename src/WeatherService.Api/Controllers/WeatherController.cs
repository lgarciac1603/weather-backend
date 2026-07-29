using Microsoft.AspNetCore.Mvc;
using WeatherService.Api.DTOs;
using WeatherService.Api.Services;

namespace WeatherService.Api.Controllers;

/// <summary>
/// Provides weather data endpoints.
/// </summary>
[ApiController]
[Route("api/weather")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;
    private readonly IGeocodingClient _geocodingClient;

    public WeatherController(IWeatherService weatherService, IGeocodingClient geocodingClient)
    {
        _weatherService = weatherService;
        _geocodingClient = geocodingClient;
    }

    /// <summary>
    /// Gets current weather data (temperature, wind direction, wind speed, and sunrise) for the given coordinates.
    /// Returns cached data if the same coordinates were already queried; otherwise fetches from Open-Meteo and caches the result.
    /// </summary>
    /// <param name="query">Latitude and longitude to query.</param>
    /// <response code="200">Weather data retrieved successfully.</response>
    /// <response code="400">Invalid latitude or longitude.</response>
    [HttpGet("coordinates")]
    [ProducesResponseType(typeof(WeatherResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WeatherResponseDto>> GetByCoordinates([FromQuery] WeatherQueryDto query)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _weatherService.GetWeatherAsync(query.Latitude, query.Longitude);
        return Ok(result);
    }

    /// <summary>
    /// Gets current weather data for the given city name.
    /// </summary>
    /// <param name="query">City name to search.</param>
    /// <response code="200">Weather data retrieved successfully.</response>
    /// <response code="400">City name is missing.</response>
    /// <response code="404">City not found.</response>
    [HttpGet("city")]
    [ProducesResponseType(typeof(WeatherResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WeatherResponseDto>> GetByCity([FromQuery] CityQueryDto query)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var coordinates = await _geocodingClient.GetCoordinatesAsync(query.City);
        if (coordinates is null)
        {
            return NotFound(new { message = $"City '{query.City}' was not found." });
        }

        var result = await _weatherService.GetWeatherAsync(coordinates.Value.Latitude, coordinates.Value.Longitude);
        return Ok(result);
    }
}