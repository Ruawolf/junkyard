public ref struct ArrayPoolOwner<T> : IDisposable
{
    private T[] array;

    private ArrayPoolOwner(T[] array)
    {
        this.array = array;
    }

    public static ArrayPoolOwner<T> Rent(int minimumLength) => new(ArrayPool<T>.Shared.Rent(minimumLength));

    public readonly T[] Array => array;

    public void Dispose()
    {
        if (array != null)
        {
            ArrayPool<T>.Shared.Return(array);
            array = null!;
        }
    }
}