using System;

namespace AppleAuth.Enums
{
    [Flags]
    public enum LoginOptions
    {
        /// <summary>
        /// Empty scope. No full name or email
        /// </summary>
        None = 0,

        /// <summary>
        /// A scope that includes the user’s full name.
        /// </summary>
        IncludeFullName = 1 << 0,

        /// <summary>
        /// A scope that includes the user’s email address
        /// </summary>
        IncludeEmail = 1 << 1,
    }
}
