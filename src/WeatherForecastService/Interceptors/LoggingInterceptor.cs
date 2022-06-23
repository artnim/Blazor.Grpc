using Grpc.Core;
using Grpc.Core.Interceptors;

namespace WeatherForecastService.Interceptors;

public partial class LoggingInterceptor : Interceptor
{
  private readonly ILogger _logger;

  public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
  {
    _logger = logger;
  }

  public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
    UnaryServerMethod<TRequest, TResponse> continuation)
  {
    GrpcCallReceived(_logger, continuation.Method.Name);
    return await continuation(request, context);
  }

  [LoggerMessage(EventId = 18_000, Message = "Received gRPC Call {Request}", Level = LogLevel.Information)]
  static partial void GrpcCallReceived(ILogger logger, string request);
}
