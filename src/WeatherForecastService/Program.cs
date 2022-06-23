using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using Serilog.Events;

var appName = ThisAssembly.AssemblyName;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", appName)
    .WriteTo.Console()
    .CreateLogger();

await Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(hostBuilder =>
    {
        hostBuilder.UseStartup<Startup>();
    })
    .UseSerilog(Log.Logger)
    .ConfigureWebHost(hostBuilder =>
    {
        hostBuilder.UseKestrel(options =>
        {
            options.ListenAnyIP(5171);
            options.ListenAnyIP(7110, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
                listenOptions.UseHttps();
            });
        });
    })
    .Build()
    .RunAsync();