namespace AppleAuth.IOS.Interfaces
{
    public interface ICredentialStateResponse
    {
        bool Success { get; }
        CredentialState CredentialState { get; }
        IAppleError Error { get; }
    }
}
