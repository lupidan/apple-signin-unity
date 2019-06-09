namespace AppleAuth.IOS.Interfaces
{
    public interface IPasswordCredential : ICredential
    {
        string Password { get; }
    }
}
