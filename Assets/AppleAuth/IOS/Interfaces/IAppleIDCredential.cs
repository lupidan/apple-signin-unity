using AppleAuth.IOS.Enums;

namespace AppleAuth.IOS.Interfaces
{
    public interface IAppleIDCredential
    {
        byte[] IdentityToken { get; }
        byte[] AuthorizationCode { get; }
        string State { get; }
        string User { get; }
        string[] AuthorizedScopes { get; }
        IPersonName FullName { get; }
        string Email { get; }
        RealUserStatus RealUserStatus { get; }
    }
}