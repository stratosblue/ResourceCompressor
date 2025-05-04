# ResourceCompressor.Test

- 编译属性 `EmbeddedResourceTesting` 用于测试嵌入文件:
  ```shell
  dotnet test -p:EmbeddedResourceTesting=true
  ```
  - 测试嵌入文件时会出现文件占用，需要解除 `ResourceCompressor` 生成文件后占用(结束相关进程)后重试
  - 
- 尝试使用如下命令运行测试
  ```shell
  dotnet build -p:EmbeddedResourceTesting=false && dotnet build && dotnet test && dotnet build -c Release -p:EmbeddedResourceTesting=false && dotnet build -c Release && dotnet test -c Release
  ```
