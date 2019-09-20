using System;

namespace AppleAuth.IOS.Enums
{
    [Flags]
    public enum LoginOptions
    {
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
