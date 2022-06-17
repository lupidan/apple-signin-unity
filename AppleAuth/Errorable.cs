using AppleAuth.Interfaces;

public class Errorable<T>
{
    public Errorable(T result)
    {
        Result = result;
    }

    public Errorable(IAppleError error)
    {
        Error = error;
    }

    public Errorable(T result, IAppleError error)
    {
        Result = result;
        Error = error;
    }

    public bool HasError => Error != null;
    public T Result { get; }
    public IAppleError Error { get; }
}
