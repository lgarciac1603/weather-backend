using Moq;
using WeatherService.Api.DTOs;
using WeatherService.Api.Models;
using WeatherService.Api.Repositories;
using WeatherService.Api.Services;
using Xunit;

namespace WeatherService.Api.Tests.Services;

public class WeatherOrchestrationServiceTests
{
    private readonly Mock<IWeatherRepository> _repositoryMock = new();
    private readonly Mock<IOpenMeteoClient> _openMeteoClientMock = new();
    private readonly WeatherOrchestrationService _sut;

    public WeatherOrchestrationServiceTests()
    {
        _sut = new WeatherOrchestrationService(_repositoryMock.Object, _openMeteoClientMock.Object);
    }

    [Fact]
    public async Task GetWeatherAsync_CacheHit_ReturnsCachedDataWithoutCallingOpenMeteo()
    {
        var cached = new WeatherRecord
        {
            Latitude = 4.6,
            Longitude = -74.1,
            Temperature = 18.5,
            WindDirection = 90,
            WindSpeed = 5.2,
            Sunrise = new DateTime(2026, 7, 29, 6, 0, 0)
        };
        _repositoryMock.Setup(r => r.GetByCoordinatesAsync(4.6, -74.1)).ReturnsAsync(cached);

        var result = await _sut.GetWeatherAsync(4.6, -74.1);

        Assert.Equal(cached.Temperature, result.Temperature);
        Assert.Equal(cached.WindDirection, result.WindDirection);
        Assert.Equal(cached.WindSpeed, result.WindSpeed);
        Assert.Equal(cached.Sunrise, result.Sunrise);
        _openMeteoClientMock.Verify(c => c.GetCurrentWeatherAsync(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        _repositoryMock.Verify(r => r.InsertAsync(It.IsAny<WeatherRecord>()), Times.Never);
    }

    [Fact]
    public async Task GetWeatherAsync_CacheMiss_FetchesFromOpenMeteoAndPersistsResult()
    {
        _repositoryMock.Setup(r => r.GetByCoordinatesAsync(6.2442, -75.5812)).ReturnsAsync((WeatherRecord?)null);
        var fetched = new WeatherResponseDto
        {
            Temperature = 22.1,
            WindDirection = 180,
            WindSpeed = 3.4,
            Sunrise = new DateTime(2026, 7, 29, 5, 45, 0)
        };
        _openMeteoClientMock.Setup(c => c.GetCurrentWeatherAsync(6.2442, -75.5812)).ReturnsAsync(fetched);

        var result = await _sut.GetWeatherAsync(6.2442, -75.5812);

        Assert.Same(fetched, result);
        _repositoryMock.Verify(r => r.InsertAsync(It.Is<WeatherRecord>(rec =>
            rec.Latitude == 6.2442 &&
            rec.Longitude == -75.5812 &&
            rec.Temperature == fetched.Temperature &&
            rec.Sunrise == fetched.Sunrise)), Times.Once);
    }

    [Fact]
    public async Task GetWeatherAsync_OpenMeteoThrows_PropagatesExceptionAndDoesNotPersist()
    {
        _repositoryMock.Setup(r => r.GetByCoordinatesAsync(It.IsAny<double>(), It.IsAny<double>())).ReturnsAsync((WeatherRecord?)null);
        _openMeteoClientMock.Setup(c => c.GetCurrentWeatherAsync(It.IsAny<double>(), It.IsAny<double>()))
            .ThrowsAsync(new HttpRequestException("Open-Meteo unavailable"));

        await Assert.ThrowsAsync<HttpRequestException>(() => _sut.GetWeatherAsync(0, 0));

        _repositoryMock.Verify(r => r.InsertAsync(It.IsAny<WeatherRecord>()), Times.Never);
    }
}
