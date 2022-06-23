using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebAssemblyApp;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddGrpcClient<WeatherForecast.Grpc.V1.WeatherForecastService.WeatherForecastServiceClient>(options =>
    {
        options.Address = new Uri("https://localhost:7110");
    })
    .ConfigurePrimaryHttpMessageHandler(() => new GrpcWebHandler(new HttpClientHandler()));

await builder.Build().RunAsync();