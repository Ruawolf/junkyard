### NativeLibraryLoad.cs
`NativeLibrary.Load`のusingラッパーです。

### Environment
- Visual Studio 2022
- .NET 8.0
- Native AOT

### Usage
使いたい関数を`UnmanagedFunctionPointer`属性つきのデリゲートとして定義します。

```cs
[UnmanagedFunctionPointer(CallingConvention.StdCall,CharSet = CharSet.Unicode)]
internal delegate int MessageBoxW(IntPtr hWnd, string lpText, string lpCaption, uint uType);
```
<br>

コンストラクタでライブラリ名を指定して、`GetProcDelegate()`の型パラメーターに定義したデリゲートを指定します。
```cs
const uint MB_OK = 0;

using var dll = new NativeLibraryLoad("User32.dll");
var MessageBoxWFunc = dll.GetProcDelegate<MessageBoxW>();

MessageBoxWFunc(IntPtr.Zero, "Message1", "Title", MB_OK);
```
<br>

`TryNativeLibraryLoad`は例外の代わりにnullを返します。
インスタンス作成時ではなく、メソッド呼び出し時にnullが返されるので注意してください。

```cs
const uint MB_OK = 0;

using var dll = new TryNativeLibraryLoad("User32.dll");
var MessageBoxWFunc = dll.GetProcDelegate<MessageBoxW>();

if (MessageBoxWFunc is not null)
    MessageBoxWFunc2(IntPtr.Zero, "Message2", "Title", MB_OK);
```
<br>

`GetProcDelegate()`に引数を渡すとデリゲート名と異なる関数名を指定できます。
```cs
dll.GetProcDelegate<MessageBoxW>("FunctionName");
```