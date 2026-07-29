using Microsoft.AspNetCore.Mvc;
using Moq;
using WeatherService.Api.Controllers;
using WeatherService.Api.DTOs;
using WeatherService.Api.Services;
using Xunit;

namespace WeatherService.Api.Tests.Controllers;

public class WeatherControllerTests
{
    private readonly Mock<IWeatherService> _weatherServiceMock = new();
    private readonly Mock<IGeocodingClient> _geocodingClientMock = new();
    private readonly WeatherController _sut;

    public WeatherControllerTests()
    {
        _sut = new WeatherController(_weatherServiceMock.Object, _geocodingClientMock.Object);
    }

    [Fact]
    public async Task GetByCoordinates_InvalidModelState_ReturnsBadRequest()
    {
        _sut.ModelState.AddModelError("Latitude", "Latitude must be between -90 and 90.");

        var result = await _sut.GetByCoordinates(new WeatherQueryDto { Latitude = 200, Longitude = 0 });

        Assert.IsType<BadRequestObjectResult>(result.Result);
        _weatherServiceMock.Verify(s => s.GetWeatherAsync(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
    }

    [Fact]
    public async Task GetByCoordinates_ValidQuery_ReturnsOkWithWeatherData()
    {
        var expected = new WeatherResponseDto { Temperature = 20, WindDirection = 90, WindSpeed = 4, Sunrise = DateTime.UtcNow };
        _weatherServiceMock.Setup(s => s.GetWeatherAsync(4.6, -74.1)).ReturnsAsync(expected);

        var result = await _sut.GetByCoordinates(new WeatherQueryDto { Latitude = 4.6, Longitude = -74.1 });

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expected, okResult.Value);
    }

    [Fact]
    public async Task GetByCity_CityNotFound_ReturnsNotFound()
    {
        _geocodingClientMock.Setup(g => g.GetCoordinatesAsync("Atlantis")).ReturnsAsync(((double, double)?)null);

        var result = await _sut.GetByCity(new CityQueryDto { City = "Atlantis" });

        Assert.IsType<NotFoundObjectResult>(result.Result);
        _weatherServiceMock.Verify(s => s.GetWeatherAsync(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
    }

    [Fact]
    public async Task GetByCity_CityFound_ReturnsOkWithWeatherData()
    {
        _geocodingClientMock.Setup(g => g.GetCoordinatesAsync("Bogota")).ReturnsAsync((4.6, -74.1));
        var expected = new WeatherResponseDto { Temperature = 18, WindDirection = 120, WindSpeed = 2, Sunrise = DateTime.UtcNow };
        _weatherServiceMock.Setup(s => s.GetWeatherAsync(4.6, -74.1)).ReturnsAsync(expected);

        var result = await _sut.GetByCity(new CityQueryDto { City = "Bogota" });

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expected, okResult.Value);
    }
}
