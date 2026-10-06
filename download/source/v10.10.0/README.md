# codeconv

Windows-1252 / ASCII 编码转换器，基于 .NET 的控制台工具（当前构建于 .NET 10）。

一个程序搞定字符与编码值的互转，支持 Windows-1252 与 ASCII 编码，既可交互使用，也能一条命令直接完成转换。

📋 更新日志：[CHANGELOG.md](./CHANGELOG.md) · [网页版](./changelog.html)

## 版本

当前版本：**10.10.0**

## 功能特性

- 字符与编码值互转（Windows-1252 / ASCII）
- 交互模式：输入编码值或字符，输入「帮助」查看内置命令
- 直接转换：一次命令完成编码或解码
- 静默模式：跳过欢迎页，适合脚本调用
- stdout / stderr 分流，出错时返回退出码 1
- 四平台合一安装包（.NET 6 / .NET 10 × x86 / x64）
- 自包含便携版：解压即用，无需安装运行时

## 用法

    codeconv [选项] [参数]

| 用法 | 说明 |
| --- | --- |
| codeconv | 进入交互模式 |
| codeconv 72 101 108 | 直接解码多个编码值 |
| codeconv --encode A | 编码单个字符 |
| codeconv --decode 72 101 108 | 显式解码（等价于直接传位置参数） |
| codeconv --version | 显示版本号 |
| codeconv --help | 显示帮助 |
| codeconv --quiet | 静默模式，跳过欢迎页 |
| codeconv --input 文件 --output 结果.txt | 文件输入输出转换 |

### 参数速查

- -v / --version：显示版本
- -h / --help：显示帮助
- -q / --quiet：静默
- -e / --encode：编码（后接一个字符或字符串）
- -d / --decode：解码（后接一个或多个编码值）
- -i / --input：读取指定文件作为输入
- -o / --output：将转换结果写入指定文件

## 安装

下载 setup.exe，在安装向导里按环境选择版本（.NET 6 / .NET 10 × x86 / x64）。如果系统未安装对应的 .NET 运行时，可在安装时勾选自动下载（默认勾选）。

## 便携版

下载对应平台的 zip，解压后直接运行 codeconv.exe。均为自包含单文件，已内置对应版本的 .NET 运行时，免安装、免依赖，适合拷贝到任意 Windows 机器上使用。

| 平台 | 文件 | 大小 |
| --- | --- | --- |
| .NET 10 · x64 | codeconv-10.10.0-portable-net10-x64.zip | 约 31 MB |
| .NET 10 · x86 | codeconv-10.10.0-portable-net10-x86.zip | 约 29 MB |
| .NET 6 · x64 | codeconv-10.10.0-portable-net6-x64.zip | 约 28 MB |
| .NET 6 · x86 | codeconv-10.10.0-portable-net6-x86.zip | 约 26 MB |

## 文件校验

| 文件 | SHA256 |
| --- | --- |
| setup.exe | d5a64f4974d47e963b7489a684878dc62c8e58041aab50c37389fde4f7dafa06 |
| codeconv-10.10.0-portable-net10-x64.zip | 3bddede87554e59ed2e5ceaf35e06133abdbdc75a1eb52c5087616fd5345a5ec |
| codeconv-10.10.0-portable-net10-x86.zip | c705a12f9c0178a31081f352e035735455d7a2cfee53336ab52156d006467e6e |
| codeconv-10.10.0-portable-net6-x64.zip | 558b9adb345d3779d1a9b0ddf8b7e0c473b63fc5e12de92818cd6a3aa35ad7d0 |
| codeconv-10.10.0-portable-net6-x86.zip | cf12b50ae1d446715a091a22a6fbe51e7ecde641afff317c94d0f89683a245e2 |

## 构建

四平台安装包：

    dotnet publish -c Release -f net10.0 -r win-x64 --self-contained false -o publish\net10.0\x64
    dotnet publish -c Release -f net10.0 -r win-x86 --self-contained false -o publish\net10.0\x86
    dotnet publish -c Release -f net6.0 -r win-x64 --self-contained false -o publish\net6.0\x64
    dotnet publish -c Release -f net6.0 -r win-x86 --self-contained false -o publish\net6.0\x86

便携版（自包含单文件）：

    dotnet publish -c Release -f net10.0 -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

## 许可

见 LICENSE。