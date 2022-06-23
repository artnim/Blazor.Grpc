using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using WeatherForecast.Grpc.V1;

namespace WeatherForecastService.Services;

public class WeatherForecastService : WeatherForecast.Grpc.V1.WeatherForecastService.WeatherForecastServiceBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public override async Task<WeatherForecastCount> GetForecastCount(WeatherForecastRequest request,
        ServerCallContext context) => await Task.FromResult(new WeatherForecastCount { Count = 50 });

    public override async Task GetForecast(WeatherForecastRequest request,
        IServerStreamWriter<WeatherForecast.Grpc.V1.WeatherForecast> responseStream, ServerCallContext context)
    {
        var weatherForecasts = Enumerable.Range(1, 50).Select(index =>
        {
            var celsius = Random.Shared.Next(-22, 55);

            return new WeatherForecast.Grpc.V1.WeatherForecast
            {
                Date = request.Date.ToDateTimeOffset().AddDays(index).ToTimestamp(),
                TemperatureC = celsius,
                TemperatureF = 32 + (int)(celsius / 0.5556),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            };
        });
        
        foreach (var forecast in request.Skip.HasValue && request.Top.HasValue
                     ? weatherForecasts.Skip(request.Skip.Value).Take(request.Top.Value)
                     : weatherForecasts)
        {
            await responseStream.WriteAsync(forecast);
        }
    }
}