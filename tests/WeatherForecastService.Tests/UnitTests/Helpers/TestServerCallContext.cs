using Grpc.Core;

namespace WeatherForecastService.Tests.UnitTests.Helpers;

public class TestServerCallContext : ServerCallContext
{
    public TestServerCallContext(Metadata requestHeadersCore, CancellationToken cancellationTokenCore,
        IDictionary<object, object> userStateCore)
    {
        RequestHeadersCore = requestHeadersCore;
        CancellationTokenCore = cancellationTokenCore;
        UserStateCore = userStateCore ?? throw new ArgumentNullException(nameof(userStateCore));
        ResponseTrailersCore = new Metadata();
        AuthContextCore = new AuthContext(string.Empty, new Dictionary<string, List<AuthProperty>>());
    }

    protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders)
    {
        if (ResponseHeaders is not null)
        {
            throw new InvalidOperationException("Response headers already written");
        }

        ResponseHeaders = responseHeaders;
        return Task.CompletedTask;
    }

    private Metadata? ResponseHeaders { get; set; }

    protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions? options)
    {
        throw new NotImplementedException();
    }

    protected override string MethodCore => "MethodName";
    protected override string HostCore => "HostName";
    protected override string PeerCore => "PeerName";
    protected override DateTime DeadlineCore => DateTime.Now.AddHours(1);
    protected override Metadata RequestHeadersCore { get; }
    protected override CancellationToken CancellationTokenCore { get; }
    protected override Metadata ResponseTrailersCore { get; }
    protected override Status StatusCore { get; set; }
    protected override WriteOptions? WriteOptionsCore { get; set; }
    protected override AuthContext AuthContextCore { get; }
    protected override IDictionary<object, object> UserStateCore { get; }

    public static TestServerCallContext Create(Metadata? requestHeaders = null,
        IDictionary<object, object>? userState = null, CancellationToken cancellationToken = default) =>
        new TestServerCallContext(requestHeaders ?? new Metadata(), cancellationToken,
            userState ?? new Dictionary<object, object>());
}