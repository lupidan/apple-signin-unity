using AppleAuth.Enums;
using AppleAuth.Interfaces;
using System;

namespace AppleAuth
{
    public interface IAppleAuthManager
    {
        void QuickLoginForAppleId(AppleAuthQuickLoginArgs quickLoginArgs, Action<ICredential> successCallback, Action<IAppleError> errorCallback);
        
        void QuickLoginForItunesKeychainCredentials(Action<ICredential> successCallback, Action<IAppleError> errorCallback);

        void LoginWithAppleId(AppleAuthLoginArgs loginArgs, Action<ICredential> successCallback, Action<IAppleError> errorCallback);

        void GetCredentialState(string userId, Action<CredentialState> successCallback, Action<IAppleError> errorCallback);

        void SetCredentialsRevokedCallback(Action<string> credentialsRevokedCallback);

        void Update();
    }
}