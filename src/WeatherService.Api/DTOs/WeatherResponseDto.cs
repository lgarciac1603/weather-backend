namespace WeatherService.Api.DTOs;

public class WeatherResponseDto
{
    public double Temperature { get; set; }
    public double WindDirection { get; set; }
    public double WindSpeed { get; set; }
    public DateTime Sunrise { get; set; }
}