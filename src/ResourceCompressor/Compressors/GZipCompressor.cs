using System.IO.Compression;

namespace ResourceCompressor.Compressors;

public class GZipCompressor : Compressor
{
    #region Public 方法

    public override void DirectCompress(Stream sourceStream, Stream outputStream)
    {
        using var gzipStream = new GZipStream(outputStream, CompressionLevel.Optimal, true);
        sourceStream.CopyTo(gzipStream);
    }

    #endregion Public 方法
}
