namespace AppleAuth.IOS.Interfaces
{
    public interface ILoginWithAppleIdResponse
    {
        bool Success { get; }
        IAppleError Error { get; }
        IAppleIDCredential AppleIDCredential { get; }
        IPasswordCredential PasswordCredential { get; }
    }
}