namespace AppleAuth.Enums
{
    /// <summary>
    /// ASAuthorizationAppleIDProvider.CredentialState
    /// </summary>
    public enum CredentialState
    {
        /// <summary>
        /// Authorization for the given user has been revoked
        /// </summary>
        Revoked = 0,

        /// <summary>
        /// The user is authorized
        /// </summary>
        Authorized = 1,

        /// <summary>
        /// The user can't be found
        /// </summary>
        NotFound = 2,
        
        /// <summary>
        /// ASAuthorizationAppleIDProviderCredentialTransferred
        /// </summary>
        Transferred = 3,
    }
}
