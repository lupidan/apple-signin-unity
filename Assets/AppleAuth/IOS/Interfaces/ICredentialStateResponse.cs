namespace AppleAuth.IOS.Interfaces
{
    public interface ICredentialStateResponse
    {
        CredentialState CredentialState { get; }
        IAppleError Error { get; }
    }
}
