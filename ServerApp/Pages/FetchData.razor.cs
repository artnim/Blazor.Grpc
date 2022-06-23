using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using WeatherForecast.Grpc.V1;

namespace ServerApp.Pages;

public partial class FetchData : ComponentBase
{
    private RadzenDataGrid<WeatherForecast.Grpc.V1.WeatherForecast> grid;
    private IList<WeatherForecast.Grpc.V1.WeatherForecast> forecasts;
    [Inject]
    private WeatherForecastService.WeatherForecastServiceClient WeatherForecastServiceClient { get; set; } = null!;

    private bool isLoading = false;

    private int count;

    protected async Task LoadData(LoadDataArgs args)
    {
        isLoading = true;

        await Task.Yield();

        count = (await WeatherForecastServiceClient
            .GetForecastCountAsync(new WeatherForecastRequest { Date = DateTimeOffset.Now.ToTimestamp() })
            .ResponseAsync).Count;

        using var call = WeatherForecastServiceClient.GetForecast(new WeatherForecastRequest
            { Date = DateTimeOffset.Now.ToTimestamp(), Skip = args.Skip, Top = args.Top });

        forecasts = new List<WeatherForecast.Grpc.V1.WeatherForecast>();

        await foreach (var forecast in call.ResponseStream.ReadAllAsync())
        {
            forecasts.Add(forecast);
        }

        isLoading = false;
    }
}