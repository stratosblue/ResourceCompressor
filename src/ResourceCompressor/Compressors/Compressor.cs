namespace ResourceCompressor.Compressors;

public abstract class Compressor : ICompressor
{
    #region Public 属性

    public virtual int MaxRetryCount { get; set; } = 5;

    #endregion Public 属性

    #region Public 方法

    public virtual bool Compress(string sourcePath, string outputPath, GenerateCompressedFileOptions generateCompressedFileOptions)
    {
        var retryCount = MaxRetryCount;
        var retryDelay = 1;
        do
        {
            try
            {
                return Compressing(sourcePath, outputPath, generateCompressedFileOptions);
            }
            catch (IOException)
            {
                //retry when parallel working
                if (retryCount < 1)
                {
                    throw;
                }
                Thread.Sleep(TimeSpan.FromSeconds(retryDelay++));
            }
        } while (retryCount-- > 0);

        throw new InvalidOperationException($"Compress file \"{sourcePath}\" to \"{outputPath}\" failed");
    }

    public abstract void DirectCompress(Stream sourceStream, Stream outputStream);

    #endregion Public 方法

    #region Protected 方法

    /// <inheritdoc cref="Compress(string, string, GenerateCompressedFileOptions)"/>
    protected bool Compressing(string sourcePath, string outputPath, GenerateCompressedFileOptions generateCompressedFileOptions)
    {
        var sourceLastWriteTime = File.GetLastWriteTimeUtc(sourcePath);

        //检查已生成文件
        if (generateCompressedFileOptions == GenerateCompressedFileOptions.PreserveNewest
            && File.Exists(outputPath))
        {
            //生成文件的时间大于原文件时间则跳过
            if (sourceLastWriteTime < File.GetLastWriteTimeUtc(outputPath))
            {
                return false;
            }
        }

        {
            using var sourceStream = File.OpenRead(sourcePath);
            using var outputStream = File.Exists(outputPath)
                                     ? File.OpenWrite(outputPath)
                                     : File.Create(outputPath);

            outputStream.SetLength(0);

            DirectCompress(sourceStream, outputStream);
        }

        return true;
    }

    #endregion Protected 方法
}
