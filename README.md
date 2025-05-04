# ResourceCompressor

基于 `MSBuild Task` 的构建时资源压缩器，用于在构建时将文件压缩后作为 `嵌入的资源` 编译到最终输出文件中。

A build time resource compressor based on the `MSBuild Task`, used to compress files and compile them as `EmbeddedResource` into the final output file during build time.

## Note:
- 读取时需要使用对应的压缩方式进行手动解压缩

## 1. 如何使用

### 1.1 安装 `ResourceCompressor` 包

```xml
<ItemGroup>
  <PackageReference Include="ResourceCompressor" Version="1.0.0">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  </PackageReference>
</ItemGroup>
```

### 1.2 在项目中使用 `CompressedEmbeddedResource` 声明需要压缩的嵌入资源

 - 简单使用时可以直接替换 `EmbeddedResource` 为 `CompressedEmbeddedResource`

```xml
<ItemGroup>
  <CompressedEmbeddedResource Include="embeddedFile.ext" />
</ItemGroup>
```

 - 默认的嵌入资源名称会在原始名称后添加压缩方式，如：`embeddedFile.ext` 使用`GZip`压缩嵌入后，资源名称会变更为 `embeddedFile.ext.gzip` 

-------

## 2. 自定义

### 2.1 `CompressedEmbeddedResource` 可选属性
#### 2.1.1 压缩算法: `CompressionAlgorithm`
```xml
<ItemGroup>
  <CompressedEmbeddedResource Include="embeddedFile.ext" CompressionAlgorithm="GZip" />
</ItemGroup>
```
- 默认值: `GZip`
- 支持的值
  - `GZip`

-------

#### 2.1.2 生成时机: `GenerateCompressedFile`
```xml
<ItemGroup>
  <CompressedEmbeddedResource Include="embeddedFile.ext" GenerateCompressedFile="PreserveNewest" />
</ItemGroup>
```
- 默认值: `Always`
- 支持的值
  - `Always`: 总是生成
  - `PreserveNewest`: 仅在文件变更时生成

-------

#### 2.1.3 生成文件后缀: `GeneratedFileSuffix`
```xml
<ItemGroup>
  <CompressedEmbeddedResource Include="embeddedFile.ext" GeneratedFileSuffix="None" />
</ItemGroup>
```
- 默认值: `AlgorithmName`
- 支持的值
  - `None`: 不添加后缀
  - `AlgorithmName`: 使用算法名称作为后缀
