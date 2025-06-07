using AppleAuth.Enums;
using AppleAuth.Interfaces;
using System;

namespace AppleAuth
{
    public interface IAppleAuthManager
    {
        [Obsolete("This method is deprecated and will be removed soon. Please provide an empty instance of AppleAuthQuickLoginArgs to QuickLogin.")]
        void QuickLogin(Action<ICredential> successCallback, Action<IAppleError> errorCallback);
        
        void QuickLogin(AppleAuthQuickLoginArgs quickLoginArgs, Action<ICredential> successCallback, Action<IAppleError> errorCallback);

        [Obsolete("This method is deprecated and will be removed soon. Please provide an instance of AppleAuthLoginArgs to LoginWithAppleId with the LoginOptions instead.")]
        void LoginWithAppleId(LoginOptions options, Action<ICredential> successCallback, Action<IAppleError> errorCallback);
        
        void LoginWithAppleId(AppleAuthLoginArgs loginArgs, Action<ICredential> successCallback, Action<IAppleError> errorCallback);

        void GetCredentialState(string userId, Action<CredentialState> successCallback, Action<IAppleError> errorCallback);

        void SetCredentialsRevokedCallback(Action<string> credentialsRevokedCallback);

        void Update();
    }
}