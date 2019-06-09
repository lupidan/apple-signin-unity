using AppleAuth.IOS.Enums;

namespace AppleAuth.IOS.Interfaces
{
    public interface IAppleIDCredential : ICredential
    {
        byte[] IdentityToken { get; }
        byte[] AuthorizationCode { get; }
        string State { get; }
        string[] AuthorizedScopes { get; }
        IPersonName FullName { get; }
        string Email { get; }
        RealUserStatus RealUserStatus { get; }
    }
}