using System.IO.Compression;
using System.Reflection;
using System.Text.Json.Nodes;
using ResourceCompressor.TestLibrary;
using ResourceCompressor.TestLibrary.MultiTargeting;

namespace ResourceCompressor.Test;

[TestClass]
public abstract class EmbeddedResourceTestBase<TargetAssemblyType>
{
    #region Protected 属性

    protected Assembly TestAssembly { get; } = typeof(TargetAssemblyType).Assembly;

    protected string TestAssemblyResourcePrefix { get; } = $"{typeof(TargetAssemblyType).Assembly.GetName().Name}.";

    #endregion Protected 属性

    #region Public 方法

#if EMBEDDED_RESOURCE_TESTING

    [TestMethod]
    [DataRow("CustomLinkDir.compressed1.test.json", "CustomLinkDir\\compressed1.test.json")]
    [DataRow("CustomLinkDir.compressed2.test.json", "CustomLinkDir\\compressed2.test.json")]
    [DataRow("CustomLinkDir.compressed3.test.json", "CustomLinkDir\\compressed3.test.json")]
    [DataRow("TestFiles.compressed.nosuffix.test.json", "TestFiles\\compressed.nosuffix.test.json")]
    [DataRow("TestFiles.compressed.test.json.gzip", "TestFiles\\compressed.test.json")]
    [DataRow("TestFiles.Dir.InnerDir1.compressed1.test.json.gzip", "TestFiles\\Dir\\InnerDir1\\compressed1.test.json")]
    [DataRow("TestFiles.Dir.InnerDir2.compressed2.test.json.gzip", "TestFiles\\Dir\\InnerDir2\\compressed2.test.json")]
    [DataRow("TestFiles.Dir.compressed3.test.json.gzip", "TestFiles\\Dir\\compressed3.test.json")]
    public void Should_Compressed_Resource_Embedded_Correctly(string embeddedResourceName, string targetLocationValue)
    {
        using var resourceStream = ReadResource(embeddedResourceName);
        using var decompressedResourceStream = Decompress(resourceStream);

        CheckResourceJsonContent(targetLocationValue, decompressedResourceStream);
    }

    [TestMethod]
    [DataRow("TestFiles.CustomLink.test.json", "TestFiles\\CustomLink\\test.json")]
    [DataRow("TestFiles.test.json", "TestFiles\\test.json")]
    public void Should_Resource_Embedded_Correctly(string embeddedResourceName, string targetLocationValue)
    {
        using var resourceStream = ReadResource(embeddedResourceName);

        CheckResourceJsonContent(targetLocationValue, resourceStream);
    }

#endif

    #endregion Public 方法

    #region Protected 方法

    protected abstract Stream Decompress(Stream stream);

    protected Stream ReadResource(string resourceName)
    {
        var result = TestAssembly.GetManifestResourceStream($"{TestAssemblyResourcePrefix}{resourceName}");
        Assert.IsNotNull(result);
        return result;
    }

    #endregion Protected 方法

    #region Private 方法

    private static void CheckResourceJsonContent(string targetLocationValue, Stream resourceStream)
    {
        var jsonObject = JsonNode.Parse(resourceStream) as JsonObject;

        Assert.IsNotNull(jsonObject);
        Assert.IsTrue(jsonObject.TryGetPropertyValue("Location", out var locationValueNode));
        Assert.IsNotNull(locationValueNode);
        Assert.AreEqual(targetLocationValue, locationValueNode.GetValue<string>());
    }

    #endregion Private 方法
}

[TestClass]
public abstract class GZipEmbeddedResourceTestBase<TargetAssemblyType> : EmbeddedResourceTestBase<TargetAssemblyType>
{
    #region Protected 方法

    protected override Stream Decompress(Stream stream)
    {
        var memoryStream = new MemoryStream((int)stream.Length);

        {
            using var gzipStream = new GZipStream(stream, CompressionMode.Decompress, true);
            gzipStream.CopyTo(memoryStream);
        }

        memoryStream.Seek(0, SeekOrigin.Begin);

        return memoryStream;
    }

    #endregion Protected 方法
}

[TestClass]
public class TestLibraryGZipEmbeddedResourceTestBase : GZipEmbeddedResourceTestBase<TestLibraryClass>
{
}

[TestClass]
public class MultiTargetingTestLibraryGZipEmbeddedResourceTestBase : GZipEmbeddedResourceTestBase<MultiTargetingTestLibraryClass>
{
}
