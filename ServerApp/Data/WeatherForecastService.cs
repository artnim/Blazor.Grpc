namespace ServerApp.Data;

public class WeatherForecastService
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public Task<IEnumerable<WeatherForecast>> GetForecastAsync(DateTime startDate, int? skip = null, int? take = null)
    {
        var weatherForecasts = Enumerable.Range(1, 50).Select(index => new WeatherForecast
        {
            Date = startDate.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        });
        
        return Task.FromResult(skip.HasValue ? weatherForecasts.Skip(skip.Value).Take(take.Value) : weatherForecasts);
    }

    public Task<int> GetForecastCountAsync() => Task.FromResult(50);
}