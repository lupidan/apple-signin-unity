namespace AppleAuth.Interfaces
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
        /// A string containing the localized description of the error.
        /// </summary>
        string LocalizedDescription { get; }

        /// <summary>
        /// An array containing the localized titles of buttons appropriate for displaying in an alert panel.
        /// </summary>
        string[] LocalizedRecoveryOptions { get; }

        /// <summary>
        /// A string containing the localized recovery suggestion for the error.
        /// </summary>
        string LocalizedRecoverySuggestion { get; }

        /// <summary>
        /// A string containing the localized explanation of the reason for the error.
        /// </summary>
        string LocalizedFailureReason { get; }
    }
}
