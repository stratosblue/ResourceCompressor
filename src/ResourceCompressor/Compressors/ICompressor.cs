namespace ResourceCompressor.Compressors;

public interface ICompressor
{
    #region Public 方法

    /// <summary>
    /// 压缩 <paramref name="sourcePath"/> 到 <paramref name="sourcePath"/>
    /// </summary>
    /// <param name="sourcePath"></param>
    /// <param name="outputPath"></param>
    /// <param name="generateCompressedFileOptions"></param>
    /// <returns>是否新生成了文件</returns>
    public bool Compress(string sourcePath, string outputPath, GenerateCompressedFileOptions generateCompressedFileOptions);

    public void DirectCompress(Stream sourceStream, Stream outputStream);

    #endregion Public 方法
}
