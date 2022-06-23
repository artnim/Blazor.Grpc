using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using WeatherForecast.Grpc.V1;

namespace WebAssemblyApp.Pages;

public partial class FetchData : ComponentBase
{
    private RadzenDataGrid<WeatherForecast> grid;
    private IList<WeatherForecast> forecasts;
    [Inject] private WeatherForecastService.WeatherForecastServiceClient WeatherForecastServiceClient { get; set; }
    private bool isLoading;
    private int count;

    protected async Task LoadData(LoadDataArgs args)
    {
        isLoading = true;
        await Task.Yield();

        count = (await WeatherForecastServiceClient
            .GetForecastCountAsync(new WeatherForecastRequest { Date = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow.Date) })
            .ResponseAsync).Count;

        using var call = WeatherForecastServiceClient.GetForecast(new WeatherForecastRequest
        {
            Date = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow.Date),
            Skip = args.Skip,
            Top = args.Top
        });

        forecasts = new List<WeatherForecast>();

        await foreach (var forecast in call.ResponseStream.ReadAllAsync())
        {
            forecasts.Add(new WeatherForecast(forecast.Date.ToDateTimeOffset(), forecast.TemperatureC,
                forecast.TemperatureF, forecast.Summary));
        }

        isLoading = false;
    }
}

public record WeatherForecast(DateTimeOffset Date, int TemperatureC, int TemperatureF, string? Summary);