namespace AppleAuth.Interfaces
{
    public interface IPasswordCredential : ICredential
    {
        string Password { get; }
    }
}
