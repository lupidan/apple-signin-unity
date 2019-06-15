using System;

namespace AppleAuth.IOS.Enums
{
    [Flags]
    public enum LoginOptions
    {
        IncludeFullName = 1 << 0,
        IncludeEmail = 1 << 1,
    }
}
