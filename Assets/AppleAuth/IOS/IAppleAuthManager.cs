using System;
using AppleAuth.IOS.Enums;
using AppleAuth.IOS.Interfaces;

namespace AppleAuth.IOS
{
    public interface IAppleAuthManager
    {
        void LoginSilently(Action<ICredential> successCallback, Action<IAppleError> errorCallback);
        void LoginWithAppleId(LoginOptions loginOptions, Action<ICredential> successCallback, Action<IAppleError> errorCallback);
        void GetCredentialState(string userId, Action<CredentialState> successCallback, Action<IAppleError> errorCallback);
    }
}