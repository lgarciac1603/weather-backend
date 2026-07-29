# weather-backend

Repo: [github.com/lgarciac1603/weather-backend](https://github.com/lgarciac1603/weather-backend)

REST API backend for querying weather data (temperature, wind direction, wind speed, and sunrise time) given latitude/longitude, using [Open-Meteo](https://open-meteo.com/en/docs) as the data source and MongoDB as a cache.

## Stack

- .NET 6 (ASP.NET Core Web API, Controllers)
- MongoDB (via `MongoDB.Driver`)
- Docker Compose to run MongoDB locally

## Project structure

```
src/WeatherService.Api/
  Controllers/   HTTP endpoints
  Services/      Business logic and external API clients (Open-Meteo forecast + geocoding)
  Repositories/  MongoDB cache access
  Models/        Persisted entities
  DTOs/          Request/response contracts
  Options/       Strongly-typed configuration
  Middleware/    Global exception handling
tests/WeatherService.Api.Tests/  Unit tests, mirroring the structure above
```

## Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) (the repo includes a `global.json` pinning the exact SDK version)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (to run MongoDB)

## Getting started

**1. Start MongoDB with Docker Compose**

```powershell
docker compose up -d
```

This starts a `mongo:6.0` container exposed on `localhost:27017`, with a named volume (`mongo_data`) to persist data between restarts.

**2. Configuration**

The MongoDB connection and the Open-Meteo API base URLs are configured in `src/WeatherService.Api/appsettings.json`:

```json
"MongoSettings": {
  "ConnectionString": "mongodb://localhost:27017",
  "DatabaseName": "WeatherServiceDb"
},
"OpenMeteoSettings": {
  "ForecastBaseUrl": "https://api.open-meteo.com/",
  "GeocodingBaseUrl": "https://geocoding-api.open-meteo.com/"
}
```

No credentials, connection strings, or URLs are hardcoded in the code — everything comes from configuration.

**3. Run the API**

```powershell
cd src/WeatherService.Api
dotnet run
```

The API is available with a self-documented Swagger UI at `/swagger`.

## Endpoints

Both endpoints are cache-first: the first request for a given location calls Open-Meteo and stores the result in MongoDB; any later request for the same location is served from MongoDB without calling the external API again.

### `GET /api/weather/coordinates`

Query parameters: `Latitude` (-90 to 90), `Longitude` (-180 to 180).

```
GET /api/weather/coordinates?Latitude=4.6&Longitude=-74.1
```

```json
{
  "temperature": 13.2,
  "windDirection": 115,
  "windSpeed": 6.4,
  "sunrise": "2026-07-28T05:53:00"
}
```

### `GET /api/weather/city` (bonus)

Query parameter: `City` (name to resolve via geocoding). Returns `404` if the city can't be found.

```
GET /api/weather/city?City=Bogota
```

Response shape is the same as the coordinates endpoint.

## Error handling

Unhandled exceptions are caught by a global middleware and returned as a consistent JSON body (`statusCode`, `message`), never as a raw stack trace. Invalid input (out-of-range coordinates, missing city) returns `400` with validation details.

## Inspecting the cache in MongoDB

To check the cached weather records directly inside the dockerized MongoDB (e.g. to verify the cache-first behavior isn't inserting duplicates):

```powershell
docker exec -it weather-service-mongo mongosh WeatherServiceDb --eval "db.WeatherRecords.find().pretty()"
```

## Running tests

Unit tests cover the cache-first orchestration logic, controller validation/routing, and the global exception middleware (common cases, edge cases, and error handling).

```powershell
dotnet test
```
