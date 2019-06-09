namespace AppleAuth.IOS.Interfaces
{
    public interface ILoginWithAppleIdResponse
    {
        bool Success { get; }
        IAppleError Error { get; }
        IAppleIDCredential AppleIDCredentials { get; }
    }
}