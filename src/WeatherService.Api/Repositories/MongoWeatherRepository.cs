using MongoDB.Driver;
using WeatherService.Api.Models;

namespace WeatherService.Api.Repositories;

/// <summary>
/// MongoDB-backed implementation of <see cref="IWeatherRepository"/>.
/// </summary>
public class MongoWeatherRepository : IWeatherRepository
{
    private readonly IMongoCollection<WeatherRecord> _collection;

    public MongoWeatherRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<WeatherRecord>("WeatherRecords");

        var indexKeys = Builders<WeatherRecord>.IndexKeys
            .Ascending(r => r.Latitude)
            .Ascending(r => r.Longitude);
        _collection.Indexes.CreateOne(new CreateIndexModel<WeatherRecord>(indexKeys));
    }

    public async Task<WeatherRecord?> GetByCoordinatesAsync(double latitude, double longitude)
    {
        // Exact double equality is safe here: latitude/longitude flow straight from the
        // query string into this comparison with no intermediate arithmetic, so the same
        // input always produces the same bit pattern.
        return await _collection
            .Find(r => r.Latitude == latitude && r.Longitude == longitude)
            .FirstOrDefaultAsync();
    }

    public async Task InsertAsync(WeatherRecord record)
    {
        await _collection.InsertOneAsync(record);
    }
}
