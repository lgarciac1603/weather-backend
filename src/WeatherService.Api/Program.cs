using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Reflection;
using WeatherService.Api.Middleware;
using WeatherService.Api.Options;
using WeatherService.Api.Repositories;
using WeatherService.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<OpenMeteoSettings>(builder.Configuration.GetSection("OpenMeteoSettings"));

builder.Services.AddHttpClient<IOpenMeteoClient, OpenMeteoClient>((sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<OpenMeteoSettings>>().Value;
    client.BaseAddress = new Uri(settings.ForecastBaseUrl);
});

builder.Services.AddHttpClient<IGeocodingClient, GeocodingClient>((sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<OpenMeteoSettings>>().Value;
    client.BaseAddress = new Uri(settings.GeocodingBaseUrl);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("MongoSettings"));

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
    var client = new MongoClient(settings.ConnectionString);
    return client.GetDatabase(settings.DatabaseName);
});

builder.Services.AddSingleton<IWeatherRepository, MongoWeatherRepository>();
builder.Services.AddScoped<IWeatherService, WeatherOrchestrationService>();

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
