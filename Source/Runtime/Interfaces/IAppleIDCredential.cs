using AppleAuth.Enums;

namespace AppleAuth.Interfaces
{
    public interface IAppleIDCredential : ICredential
    {
        /// <summary>
        /// A JSON Web Token (JWT) that securely communicates information about the user to your app.
        /// </summary>
        byte[] IdentityToken { get; }

        /// <summary>
        /// A short-lived token used by your app for proof of authorization when interacting with the app’s server counterpart.
        /// </summary>
        byte[] AuthorizationCode { get; }

        /// <summary>
        /// An arbitrary string that your app provided to the request that generated the credential.
        /// </summary>
        string State { get; }

        /// <summary>
        /// The contact information the user authorized your app to access.
        /// </summary>
        string[] AuthorizedScopes { get; }

        /// <summary>
        /// The user’s name
        /// </summary>
        IPersonName FullName { get; }

        /// <summary>
        /// The user’s email address
        /// </summary>
        string Email { get; }

        /// <summary>
        /// A value that indicates whether the user appears to be a real person.
        /// </summary>
        RealUserStatus RealUserStatus { get; }
    }
}