using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using WeatherForecast.Grpc.V1;
using WeatherForecastService.Tests.UnitTests.Helpers;

namespace WeatherForecastService.Tests.UnitTests;

public class WeatherForecastServiceTests
{
    private readonly Services.WeatherForecastService _weatherForecastService;

    public WeatherForecastServiceTests()
    {
        _weatherForecastService = new Services.WeatherForecastService();
    }

    [Fact]
    public async Task GetWeatherForecastCount_Returns_Correct_Count()
    {
        var result = (await _weatherForecastService.GetForecastCount( new WeatherForecastRequest{ Date = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime())}, TestServerCallContext.Create())).Count;

        result.Should().Be(50);
    }
}