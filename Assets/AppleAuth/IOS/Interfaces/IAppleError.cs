namespace AppleAuth.IOS.Interfaces
{
    public interface IAppleError
    {
        /// <summary>
        /// The error code.
        /// </summary>
        int Code { get; }

        /// <summary>
        /// A string containing the error domain.
        /// </summary>
        string Domain { get; }

        /// <summary>
        /// Retrieve the localized description for this error
        /// </summary>
        string LocalizedDescription { get; }

        /// <summary>
        /// Provides a set of possible recovery options to present to the user
        /// </summary>
        string[] LocalizedRecoveryOptions { get; }

        /// <summary>
        /// "Default implementation provided"
        /// </summary>
        string LocalizedRecoverySuggestion { get; }

        /// <summary>
        /// "Default implementation provided"
        /// </summary>
        string LocalizedFailureReason { get; }
    }
}
