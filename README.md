# weather-backend

REST API backend for querying weather data (temperature, wind direction, wind speed, and sunrise time) given latitude/longitude, using [Open-Meteo](https://open-meteo.com/en/docs) as the data source and MongoDB as a cache.

## Stack

- .NET 6 (ASP.NET Core Web API, Controllers)
- MongoDB (via `MongoDB.Driver`)
- Docker Compose to run MongoDB locally

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

The MongoDB connection is configured in `src/WeatherService.Api/appsettings.json`, under the `MongoSettings` section:

```json
"MongoSettings": {
  "ConnectionString": "mongodb://localhost:27017",
  "DatabaseName": "WeatherServiceDb"
}
```

No credentials or sensitive values are hardcoded in the code — everything comes from configuration.

**3. Run the API**

```powershell
cd src/WeatherService.Api
dotnet run
```

The API is available with a self-documented Swagger UI at `/swagger`.

## Project status

Actively in development. So far: MongoDB connection via DI, domain model (`WeatherRecord`) and DTOs, typed HTTP client for Open-Meteo (`IOpenMeteoClient`), cache-first repository (`IWeatherRepository`). Pending: main endpoint, error handling, and the bonus city-based endpoint.
