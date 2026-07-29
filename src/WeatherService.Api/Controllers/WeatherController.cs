using Microsoft.AspNetCore.Mvc;
using WeatherService.Api.DTOs;
using WeatherService.Api.Services;

namespace WeatherService.Api.Controllers;

[ApiController]
[Route("api/weather")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet("coordinates")]
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