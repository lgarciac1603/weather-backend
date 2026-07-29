using Microsoft.AspNetCore.Mvc;
using WeatherService.Api.DTOs;
using WeatherService.Api.Services;

/// <summary>
/// Provides weather data endpoints.
/// </summary>
[ApiController]
[Route("api/weather")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
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
}