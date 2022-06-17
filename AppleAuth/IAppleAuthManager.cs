using AppleAuth.Enums;
using AppleAuth.Interfaces;
using System;
using System.Threading.Tasks;

namespace AppleAuth
{
    public interface IAppleAuthManager
    {
        [Obsolete("This method is deprecated and will be removed soon. Please provide an empty instance of AppleAuthQuickLoginArgs to QuickLogin.")]
        void QuickLogin(Action<ICredential> successCallback, Action<IAppleError> errorCallback);
        
        void QuickLogin(AppleAuthQuickLoginArgs quickLoginArgs, Action<ICredential> successCallback, Action<IAppleError> errorCallback);

        Task<Errorable<ICredential>> QuickLoginAsync(AppleAuthQuickLoginArgs quickLoginArgs);

        [Obsolete("This method is deprecated and will be removed soon. Please provide an instance of AppleAuthLoginArgs to LoginWithAppleId with the LoginOptions instead.")]
        void LoginWithAppleId(LoginOptions options, Action<ICredential> successCallback, Action<IAppleError> errorCallback);
        
        void LoginWithAppleId(AppleAuthLoginArgs loginArgs, Action<ICredential> successCallback, Action<IAppleError> errorCallback);

        Task<Errorable<ICredential>> LoginWithAppleIdAsync(AppleAuthLoginArgs loginArgs);

        void GetCredentialState(string userId, Action<CredentialState> successCallback, Action<IAppleError> errorCallback);

        Task<Errorable<CredentialState>> GetCredentialStateAsync(string userId);

        void SetCredentialsRevokedCallback(Action<string> credentialsRevokedCallback);

        void Update();

        Task<string> WaitForCredentialsRevokedAsync();
    }
}