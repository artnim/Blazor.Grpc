using System.Security.Authentication;
using Grpc.Core;

namespace WeatherForecastService.Interceptors.Helpers;

public static partial class ExceptionHelpers
{
  public static RpcException Handle<T>(this Exception exception, ServerCallContext context, ILogger<T> logger,
    string traceId) =>
    exception switch
    {
      TimeoutException timeoutException => HandleTimeoutException(timeoutException, context, logger, traceId),
      AuthenticationException authenticationException => HandleAuthenticationException(authenticationException, context,
        logger, traceId),
      _ => HandleDefault(exception, context, logger, traceId)
    };

  private static RpcException HandleAuthenticationException<T>(AuthenticationException authenticationException,
    ServerCallContext context, ILogger<T> logger, string traceId)
  {
    AuthenticationError(logger, traceId);

    return new RpcException(new Status(StatusCode.Unauthenticated, authenticationException.Message),
      CreateTrailers(traceId));
  }

  private static RpcException HandleTimeoutException<T>(TimeoutException exception, ServerCallContext _,
    ILogger<T> logger, string traceId)
  {
    TimeoutError(logger, traceId);

    var status = new Status(StatusCode.Internal, "An external resource did not answer within the time limit");
    return new RpcException(status, CreateTrailers(traceId));
  }

  private static RpcException HandleDefault<T>(Exception exception, ServerCallContext context, ILogger<T> logger,
    string traceId)
  {
    Error(logger, traceId);

    return new RpcException(new Status(StatusCode.Internal, exception.Message), CreateTrailers(traceId));
  }

  private static Metadata CreateTrailers(string traceId) => new() { { "CorrelationId", traceId } };

  [LoggerMessage(EventId = 18_001, Message = "TraceId: {TraceId} - Authentication error",
    Level = LogLevel.Error)]
  public static partial void AuthenticationError(ILogger logger, string traceId);

  [LoggerMessage(EventId = 18_002, Message = "TraceId: {TraceId} - A timeout occurred", Level = LogLevel.Error)]
  public static partial void TimeoutError(ILogger logger, string traceId);

  [LoggerMessage(EventId = 18_003, Message = "TraceId: {TraceId} - An error occurred", Level = LogLevel.Error)]
  public static partial void Error(ILogger logger, string traceId);
}
