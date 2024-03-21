using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ruawolf;

public static class BinaryReaderExtension
{
    public static bool TryReadStringStyleChars(this BinaryReader reader, Span<char> charSpan, out int charsSize)
    {
        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_encoding")]
        static extern Encoding GetEncoding(BinaryReader reader);

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_decoder")]
        static extern ref Decoder? GetDecoder(BinaryReader reader);


        var charBytesSize = reader.Read7BitEncodedInt();

        if (charBytesSize < 0)
            throw new IOException($"BinaryReader encountered an invalid string length of {charBytesSize} characters.");

        if (charBytesSize == 0)
        {
            charsSize = 0;
            return true;
        }

        var byteBuffer = ArrayPool<byte>.Shared.Rent(charBytesSize);
        var byteSpan = byteBuffer.AsSpan()[..charBytesSize];

        var readBytesSize = reader.Read(byteSpan);

        if (readBytesSize == 0)
            throw new EndOfStreamException("Unable to read beyond the end of the stream.");

        if (readBytesSize != charBytesSize)
            throw new IOException("Requested size could not be read from stream.");


        ref var decoder = ref GetDecoder(reader);
        decoder ??= GetEncoding(reader).GetDecoder();

        decoder.Convert(byteSpan, charSpan, false, out _, out var charsUsed, out var completed);
        charsSize = completed ? charsUsed : decoder.GetCharCount(byteSpan, false);

        ArrayPool<byte>.Shared.Return(byteBuffer);

        if (!completed)
            reader.BaseStream.Position -= readBytesSize;

        return completed;
    }
}