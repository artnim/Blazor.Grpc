using System.IO.Compression;
using Grpc.Net.Compression;

namespace WeatherForecastService.Compression;

public class BrotliCompressionProvider : ICompressionProvider
{
  private readonly CompressionLevel? _compressionLevel;

  public BrotliCompressionProvider(CompressionLevel compressionLevel)
  {
    _compressionLevel = compressionLevel;
  }

  public BrotliCompressionProvider()
  {
  }

  public Stream CreateCompressionStream(Stream stream, CompressionLevel? compressionLevel) => new BrotliStream(stream,
    compressionLevel ?? _compressionLevel ?? CompressionLevel.Fastest, true);

  public Stream CreateDecompressionStream(Stream stream) => new BrotliStream(stream, CompressionMode.Decompress);

  public string EncodingName => "br";
}
