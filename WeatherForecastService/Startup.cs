using System.IO.Compression;
using Grpc.Net.Compression;
using WeatherForecastService.Compression;
using WeatherForecastService.Interceptors;

public class Startup
{
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

        // Add services to the container.
        services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = true;
            options.MaxReceiveMessageSize = 0x600000;
            options.MaxSendMessageSize = 0x600000;
            options.IgnoreUnknownServices = true;
            options.CompressionProviders = new List<ICompressionProvider>
            {
                new GzipCompressionProvider(CompressionLevel.Optimal),
                new BrotliCompressionProvider(CompressionLevel.Optimal)
            };
            options.ResponseCompressionAlgorithm = "br";
            options.ResponseCompressionLevel = CompressionLevel.Optimal;
            options.Interceptors.Add<ExceptionInterceptor>();
            options.Interceptors.Add<LoggingInterceptor>();
        });

        services.AddGrpcReflection();

        services.AddCors(options => options.AddPolicy("AllowAll", policyBuilder =>
        {
            policyBuilder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
        }));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

        app.UseCors();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<WeatherForecastService.Services.WeatherForecastService>().EnableGrpcWeb()
                .RequireCors("AllowAll");

            endpoints.MapGrpcReflectionService();
        });
    }
}