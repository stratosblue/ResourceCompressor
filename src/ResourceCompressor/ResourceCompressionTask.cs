#pragma warning disable CS8618

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using ResourceCompressor.Compressors;
using ResourceCompressor.Extensions;
using ResourceCompressor.Util;

namespace ResourceCompressor;

public class ResourceCompressionTask : Microsoft.Build.Utilities.Task
{
    #region Public 属性

    [Required]
    public string IntermediateOutputPath { get; set; }

    [Output]
    public ITaskItem[] OutputFiles { get; set; }

    [Required]
    public ITaskItem[] SourceFiles { get; set; }

    /// <summary>
    /// 工作类型(暂时不清楚targets如何直接传递枚举)
    /// </summary>
    [Required]
    public string WorkType { get; set; }

    #endregion Public 属性

    #region Public 方法

    /// <inheritdoc/>
    public override bool Execute()
    {
        if (!SourceFiles.Any())
        {
            return true;
        }

        var workType = ParseUtil.ParseEnumValue<CompressionWorkType>(WorkType);

        switch (workType)
        {
            case CompressionWorkType.CompressedEmbeddedResource:
                OutputFiles = CompressionCompressedEmbeddedResource(SourceFiles, IntermediateOutputPath);
                break;

            default:
                throw new InvalidOperationException($"Unsupported {nameof(CompressionWorkType)} - \"{WorkType}\"");
        }

        return true;
    }

    #endregion Public 方法

    #region Private 方法

    /// <summary>
    /// 压缩 压缩嵌入资源
    /// </summary>
    /// <param name="sourceFiles"></param>
    /// <param name="intermediateOutputPath"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static ITaskItem[] CompressionCompressedEmbeddedResource(ITaskItem[] sourceFiles, string intermediateOutputPath)
    {
        var outputDirectory = Path.Combine(intermediateOutputPath, "ResourceCompressor", nameof(CompressionWorkType.CompressedEmbeddedResource));
        DirectoryUtil.EnsureDirectory(outputDirectory);

        var outputFiles = new ITaskItem[sourceFiles.Length];

        for (var i = 0; i < sourceFiles.Length; i++)
        {
            //获取选项
            var sourceFile = sourceFiles[i];

            var sourceFilePath = sourceFile.GetMetadata("FullPath");
            var relativeDir = sourceFile.GetMetadata("RelativeDir");

            var compressionAlgorithm = sourceFile.GetTypedMetadata("CompressionAlgorithm", CompressionAlgorithm.GZip);
            var generateCompressedFileOptions = sourceFile.GetTypedMetadata("GenerateCompressedFile", GenerateCompressedFileOptions.Always);
            var generatedFileFormatOptions = sourceFile.GetMetadata("GeneratedFileFormat") ?? string.Empty;

            if (string.IsNullOrWhiteSpace(generatedFileFormatOptions))
            {
                generatedFileFormatOptions = "{0}";
            }

            //确认生成文件名
            var fileName = string.Format(generatedFileFormatOptions, Path.GetFileName(sourceFilePath));

            //确认输出目录
            var outputFilePath = Path.Combine(outputDirectory, relativeDir, fileName);
            DirectoryUtil.EnsureDirectory(Path.GetDirectoryName(outputFilePath)!);

            var outputFile = outputFiles[i] = new TaskItem(outputFilePath);
            sourceFile.CopyMetadataTo(outputFile);
            //用于嵌入资源名称
            var link = sourceFile.GetMetadata("Link");  //获取原始的Link
            if (string.IsNullOrWhiteSpace(link))
            {
                //避免 ./ 等导致最终嵌入路径有过多的 .
                relativeDir = Path.GetFullPath(relativeDir);
                relativeDir = relativeDir.Substring(Environment.CurrentDirectory.Length).TrimStart(['\\', '/']);
                link = Path.Combine(relativeDir, fileName);
            }
            outputFile.SetMetadata("ResourceCompressorEmbeddedLinkPath", link);

            //压缩文件
            var compressor = GetCompressor(compressionAlgorithm);
            compressor.Compress(sourceFilePath, outputFilePath, generateCompressedFileOptions);
        }

        return outputFiles;
    }

    private static ICompressor GetCompressor(CompressionAlgorithm compressionAlgorithm)
    {
        return compressionAlgorithm switch
        {
            CompressionAlgorithm.GZip => new GZipCompressor(),
            _ => throw new InvalidOperationException($"Unsupported {nameof(CompressionAlgorithm)} - \"{compressionAlgorithm}\"")
        };
    }

    #endregion Private 方法
}
