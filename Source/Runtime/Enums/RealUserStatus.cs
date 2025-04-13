namespace AppleAuth.Enums
{
    /// <summary>
    /// ASUserDetectionStatus
    /// </summary>
    public enum RealUserStatus
    {
        /// <summary>
        /// The system can't determine this user's status as a real person.
        /// </summary>
        Unsupported = 0,

        /// <summary>
        /// The system hasn't determined whether the user might be a real person.
        /// </summary>
        Unknown = 1,

        /// <summary>
        /// The user appears to be a real person.
        /// </summary>
        LikelyReal = 2,
    }
}
