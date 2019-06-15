using System;
using AppleAuth.IOS.Enums;
using AppleAuth.IOS.Interfaces;

namespace AppleAuth.IOS.Extensions
{
    public static class AppleErrorExtensions
    {
        public static AuthorizationErrorCode GetAuthorizationErrorCode(this IAppleError error)
        {
            if (error.Domain == "com.apple.AuthenticationServices.AuthorizationError" &&
                Enum.IsDefined(typeof(AuthorizationErrorCode), error.Code))
            {
                return (AuthorizationErrorCode)error.Code;
            }
            
            return AuthorizationErrorCode.Unknown;
        }
    }
}
