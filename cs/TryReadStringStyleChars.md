### TryReadStringStyleChars.cs
`BinaryReader.ReadString()`のゼロアロケーション代替です。

> [!NOTE]
> `BinaryReader.ReadChars()`は`ReadString()`と互換がなく、`byte[]`を返すメソッドしかありません。

### Environment
- .NET 8.0

### Usage

#### パラメーター
`Span<char> charSpan` 書き込み用バッファ
</br>
`out int charsSize` 読み取れた文字数

#### 戻り値
`bool` 書き込み用バッファに全ての文字が書き込めた場合は`true`

```cs
private char[] charBuffer = new char[64];

for (int i = 0; i < count; i++)
{
    if (!reader.TryReadStringStyleChars(charBuffer, out var charsSize))
    {
        charBuffer = new char[charsSize * 2];
        i--;
        continue;
    }

    var charsSpan = charBuffer.AsSpan()[..charsSize];
}
```
<br>
