using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WeatherService.Api.Models;

public class WeatherRecord
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Temperature { get; set; }
    public double WindDirection { get; set; }
    public double WindSpeed { get; set; }
    public DateTime Sunrise { get; set; }
    public DateTime FetchedAt { get; set; }
}