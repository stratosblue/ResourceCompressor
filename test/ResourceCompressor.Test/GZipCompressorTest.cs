using System.IO.Compression;
using System.Security.Cryptography;
using ResourceCompressor.Compressors;

namespace ResourceCompressor.Test;

[TestClass]
public class GZipCompressorTest : CompressorTestBase
{
    #region Public 方法

    protected override StreamWithSHA256 Decompress(Stream stream)
    {
        var memoryStream = new MemoryStream((int)stream.Length);

        {
            using var gzipStream = new GZipStream(stream, CompressionMode.Decompress, true);
            gzipStream.CopyTo(memoryStream);
        }

        memoryStream.Seek(0, SeekOrigin.Begin);

        return new(memoryStream, (int)memoryStream.Length, SHA256.HashData(memoryStream));
    }

    protected override ICompressor GetCompressor() => new GZipCompressor();

    #endregion Public 方法
}
