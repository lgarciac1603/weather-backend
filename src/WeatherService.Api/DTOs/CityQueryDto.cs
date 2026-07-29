using System.ComponentModel.DataAnnotations;

namespace WeatherService.Api.DTOs;

public class CityQueryDto
{
    [Required(ErrorMessage = "City name is required.")]
    public string City { get; set; } = string.Empty;
}