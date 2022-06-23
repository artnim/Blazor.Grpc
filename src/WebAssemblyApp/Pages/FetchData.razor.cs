using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using WeatherForecast.Grpc.V1;

namespace WebAssemblyApp.Pages;

public partial class FetchData : ComponentBase
{
    private RadzenDataGrid<WeatherForecast.Grpc.V1.WeatherForecast>? _grid;
    private IList<WeatherForecast.Grpc.V1.WeatherForecast>? _forecasts;
    [Inject] private WeatherForecastService.WeatherForecastServiceClient WeatherForecastServiceClient { get; set; } = null!;
    private bool _isLoading;
    private int _count;

    private async Task LoadData(LoadDataArgs args)
    {
        _isLoading = true;
        await Task.Yield();

        _count = (await WeatherForecastServiceClient
            .GetForecastCountAsync(new WeatherForecastRequest { Date = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow.Date) })
            .ResponseAsync).Count;

        using var call = WeatherForecastServiceClient.GetForecast(new WeatherForecastRequest
        {
            Date = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow.Date),
            Skip = args.Skip,
            Top = args.Top
        });

        _forecasts = new List<WeatherForecast.Grpc.V1.WeatherForecast>();

        await foreach (var forecast in call.ResponseStream.ReadAllAsync())
        {
            _forecasts.Add(forecast);
        }

        _isLoading = false;
    }
}