using System.Security.Cryptography;
using ResourceCompressor.Compressors;

namespace ResourceCompressor.Test;

[TestClass]
public abstract class CompressorTestBase
{
    #region Public 方法

    [TestMethod]
    public void Should_Compress_Success()
    {
        var compressor = GetCompressor();

        Assert.IsNotNull(compressor);

        var (stream, length, hash) = GetTestData();

        var testSourceFile = Path.GetTempFileName();
        var testOutputFile = Path.GetTempFileName();

        try
        {
            {
                using var testSourceFileStream = File.OpenWrite(testSourceFile);
                stream.CopyTo(testSourceFileStream);
            }

            var isNewFile = compressor.Compress(testSourceFile, testOutputFile, GenerateCompressedFileOptions.Always);
            Assert.IsTrue(isNewFile);

            Assert.AreNotEqual(0, new FileInfo(testOutputFile).Length);

            var (decompressedStream, decompressedLength, decompressedHash) = Decompress(testOutputFile);

            Assert.AreEqual(length, decompressedLength);
            CollectionAssert.AreEqual(hash, decompressedHash);
        }
        finally
        {
            try
            {
                File.Delete(testSourceFile);
            }
            catch { }

            try
            {
                File.Delete(testOutputFile);
            }
            catch { }
        }
    }

    [TestMethod]
    public void Should_Compress_PreserveNewest_Success()
    {
        var compressor = GetCompressor();

        Assert.IsNotNull(compressor);

        var (stream, length, hash) = GetTestData();

        var testSourceFile = Path.GetTempFileName();
        var testOutputFile = Path.GetTempFileName();

        try
        {
            {
                using var testSourceFileStream = File.OpenWrite(testSourceFile);
                stream.CopyTo(testSourceFileStream);
            }

            var isNewFile = compressor.Compress(testSourceFile, testOutputFile, GenerateCompressedFileOptions.Always);
            Assert.IsTrue(isNewFile);

            Assert.AreNotEqual(0, new FileInfo(testOutputFile).Length);

            var (decompressedStream, decompressedLength, decompressedHash) = Decompress(testOutputFile);

            Assert.AreEqual(length, decompressedLength);
            CollectionAssert.AreEqual(hash, decompressedHash);

            //占用并读共享
            using var fileStream = File.Open(testOutputFile, FileMode.Open, FileAccess.Write, FileShare.Read);
            isNewFile = compressor.Compress(testSourceFile, testOutputFile, GenerateCompressedFileOptions.PreserveNewest);
            Assert.IsFalse(isNewFile);
        }
        finally
        {
            try
            {
                File.Delete(testSourceFile);
            }
            catch { }

            try
            {
                File.Delete(testOutputFile);
            }
            catch { }
        }
    }


    [TestMethod]
    public void Should_DirectCompress_Success()
    {
        var compressor = GetCompressor();

        Assert.IsNotNull(compressor);

        var (stream, length, hash) = GetTestData();

        using var testStream = new MemoryStream(length);
        compressor.DirectCompress(stream, testStream);

        Assert.AreNotEqual(0, testStream.Length);

        testStream.Seek(0, SeekOrigin.Begin);

        var (decompressedStream, decompressedLength, decompressedHash) = Decompress(testStream);

        Assert.AreEqual(length, decompressedLength);
        CollectionAssert.AreEqual(hash, decompressedHash);
    }

    protected abstract StreamWithSHA256 Decompress(Stream stream);

    protected virtual StreamWithSHA256 Decompress(string filePath)
    {
        using var fileStream = File.OpenRead(filePath);
        return Decompress(fileStream);
    }

    protected abstract ICompressor GetCompressor();

    #endregion Public 方法

    #region Protected 方法

    protected static StreamWithSHA256 GetTestData(int dataLength = 10240)
    {
        var data = new byte[dataLength];
        Random.Shared.NextBytes(data);
        return new(new MemoryStream(data), dataLength, SHA256.HashData(data));
        throw new NotImplementedException();
    }

    #endregion Protected 方法
}

public record struct StreamWithSHA256(Stream Stream, int Length, byte[] SHA256);
