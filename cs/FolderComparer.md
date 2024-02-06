### FolderComparer.cs
指定したフォルダー下の全ファイルをバイナリレベルで比較します。

> [!NOTE]
> ファイル名が違うとその時点で別ファイルとして扱われます。

### Environment
- .NET 8.0
- Native AOT

### Usage

ビルドして出来たEXEの引数にフォルダーパスを2つ渡してください。

```cmd
> FolderComparer <FolderPath1> <FolderPath2>
```

### Memo

リザルト表示のソート実装が面倒くさいので、ファイル比較の並列化は保留。

