using System;
using AppleAuth.IOS.Enums;
using AppleAuth.IOS.Interfaces;

namespace AppleAuth.IOS
{
    public class AppleAuthManager : IAppleAuthManager
    {
#if UNITY_IOS && !UNITY_EDITOR
        private readonly IPayloadDeserializer _payloadDeserializer;
        private readonly IMessageHandlerScheduler _scheduler;
        
        private uint _registeredCredentialsRevokedCallbackId;
        private bool _didRegisterCredentialsRevokedCallback = false;
#endif

        public bool IsCurrentPlatformSupported
        {
            get
            {
#if UNITY_IOS && !UNITY_EDITOR
                return PInvoke.AppleAuth_IOS_IsCurrentPlatformSupported();
#else
                return false;
#endif
            }
        }

        public AppleAuthManager(IPayloadDeserializer payloadDeserializer, IMessageHandlerScheduler scheduler)
        {
#if UNITY_IOS && !UNITY_EDITOR
            this._payloadDeserializer = payloadDeserializer;
            this._scheduler = scheduler;
#endif
        }
        
        public void LoginSilently(
            Action<ICredential> successCallback,
            Action<IAppleError> errorCallback)
        {
#if UNITY_IOS && !UNITY_EDITOR
            var requestId = NativeMessageHandler.AddMessageCallback(
                this._scheduler,
                true,
                payload =>
                {
                    var response = this._payloadDeserializer.DeserializeLoginWithAppleIdResponse(payload);
                    if (response.Error != null)
                        errorCallback(response.Error);
                    else if (response.PasswordCredential != null)
                        successCallback(response.PasswordCredential);
                    else
                        successCallback(response.AppleIDCredential);
                });

            PInvoke.AppleAuth_IOS_LoginSilently(requestId);
#else
            throw new Exception("Apple Auth is only supported for iOS 13.0 onwards");
#endif
        }
        
        public void LoginWithAppleId(
            LoginOptions loginOptions,
            Action<ICredential> successCallback,
            Action<IAppleError> errorCallback)
        {
#if UNITY_IOS && !UNITY_EDITOR
            var requestId = NativeMessageHandler.AddMessageCallback(
                this._scheduler,
                true,
                payload =>
                {
                    var response = this._payloadDeserializer.DeserializeLoginWithAppleIdResponse(payload);
                    if (response.Error != null)
                        errorCallback(response.Error);
                    else
                        successCallback(response.AppleIDCredential);
                });
            
            PInvoke.AppleAuth_IOS_LoginWithAppleId(requestId, (int)loginOptions);
#else
            throw new Exception("Apple Auth is only supported for iOS 13.0 onwards");
#endif
        }
        
        public void GetCredentialState(
            string userId,
            Action<CredentialState> successCallback,
            Action<IAppleError> errorCallback)
        {
#if UNITY_IOS && !UNITY_EDITOR
            var requestId = NativeMessageHandler.AddMessageCallback(
                this._scheduler,
                true,
                payload =>
                {
                    var response = this._payloadDeserializer.DeserializeCredentialStateResponse(payload);
                    if (response.Error != null)
                        errorCallback(response.Error);
                    else
                        successCallback(response.CredentialState);
                });
            
            PInvoke.AppleAuth_IOS_GetCredentialState(requestId, userId);
#else
            throw new Exception("Apple Auth is only supported for iOS 13.0 onwards");
#endif
        }
        
        public void SetCredentialsRevokedCallback(Action<string> credentialsRevokedCallback)
        {
#if UNITY_IOS && !UNITY_EDITOR
            if (this._didRegisterCredentialsRevokedCallback)
            {
                NativeMessageHandler.ReplaceMessageCallback(
                    this._registeredCredentialsRevokedCallbackId,
                    this._scheduler,
                    false,
                    credentialsRevokedCallback);
            }
            else
            {
                this._registeredCredentialsRevokedCallbackId = NativeMessageHandler.AddMessageCallback(
                    this._scheduler,
                    false,
                    credentialsRevokedCallback);
                
                this._didRegisterCredentialsRevokedCallback = true;
                PInvoke.AppleAuth_IOS_RegisterCredentialsRevokedCallbackId(this._registeredCredentialsRevokedCallbackId);
            }
#else
            throw new Exception("Apple Auth is only supported for iOS 13.0 onwards");
#endif
        }

#if UNITY_IOS && !UNITY_EDITOR
        private static class PInvoke
        {
            [System.Runtime.InteropServices.DllImport("__Internal")]
            public static extern bool AppleAuth_IOS_IsCurrentPlatformSupported();
            
            [System.Runtime.InteropServices.DllImport("__Internal")]
            public static extern void AppleAuth_IOS_GetCredentialState(uint requestId, string userId);

            [System.Runtime.InteropServices.DllImport("__Internal")]
            public static extern void AppleAuth_IOS_LoginWithAppleId(uint requestId, int loginOptions);
            
            [System.Runtime.InteropServices.DllImport("__Internal")]
            public static extern void AppleAuth_IOS_LoginSilently(uint requestId);
            
            [System.Runtime.InteropServices.DllImport("__Internal")]
            public static extern void AppleAuth_IOS_RegisterCredentialsRevokedCallbackId(uint callbackId);
        }
#endif
    }
}
