using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Reflection;
using WeatherService.Api.Middleware;
using WeatherService.Api.Options;
using WeatherService.Api.Repositories;
using WeatherService.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<IOpenMeteoClient, OpenMeteoClient>(client =>
{
    client.BaseAddress = new Uri("https://api.open-meteo.com/");
});

builder.Services.AddHttpClient<IGeocodingClient, GeocodingClient>(client =>
{
    client.BaseAddress = new Uri("https://geocoding-api.open-meteo.com/");
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// register mongo config
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
