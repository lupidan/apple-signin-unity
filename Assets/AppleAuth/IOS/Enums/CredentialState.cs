namespace AppleAuth.IOS.Enums
{
    /// <summary>
    /// ASAuthorizationAppleIDProvider.CredentialState
    /// </summary>
    public enum CredentialState
    {
        Unknown = -1,

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
    }
}
