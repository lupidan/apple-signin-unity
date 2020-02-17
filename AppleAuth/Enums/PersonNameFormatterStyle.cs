namespace AppleAuth.Enums
{
    /// <summary>
    /// NSPersonNameComponentsFormatter
    /// </summary>
    public enum PersonNameFormatterStyle
    {
        /// <summary>
        /// The minimally necessary features for differentiation in a casual setting. Equivalent to NSPersonNameComponentsFormatterStyleMedium.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Relies on user preferences and language defaults to display shortened form appropriate for display in space-constrained settings.
        /// </summary>
        Short = 1,

        /// <summary>
        /// The minimally necessary features for differentiation in a casual setting. Equivalent to NSPersonNameComponentsFormatterStyleDefault.
        /// </summary>
        Medium = 2,

        /// <summary>
        /// The fully qualified name complete with all known components.
        /// </summary>
        Long = 3,

        /// <summary>
        /// The maximally abbreviated form of a name.
        /// </summary>
        Abbreviated = 4,
    }
}
