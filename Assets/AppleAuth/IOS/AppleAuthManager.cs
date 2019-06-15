using System;
using System.Runtime.InteropServices;
using AppleAuth.IOS.Enums;
using AppleAuth.IOS.Interfaces;

namespace AppleAuth.IOS
{
    public class AppleAuthManager : IAppleAuthManager
    {
        private readonly IPayloadDeserializer _payloadDeserializer;
        private readonly IMessageHandlerScheduler _scheduler;

        public AppleAuthManager(IPayloadDeserializer payloadDeserializer, IMessageHandlerScheduler scheduler)
        {
            this._payloadDeserializer = payloadDeserializer;
            this._scheduler = scheduler;
        }
        
        public void LoginSilently(
            Action<ICredential> successCallback,
            Action<IAppleError> errorCallback)
        {
            var requestId = NativeMessageHandler.AddMessageCallback(
                this._scheduler,
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
        }
        
        public void LoginWithAppleId(
            LoginOptions loginOptions,
            Action<ICredential> successCallback,
            Action<IAppleError> errorCallback)
        {
            var requestId = NativeMessageHandler.AddMessageCallback(
                this._scheduler,
                payload =>
                {
                    var response = this._payloadDeserializer.DeserializeLoginWithAppleIdResponse(payload);
                    if (response.Error != null)
                        errorCallback(response.Error);
                    else
                        successCallback(response.AppleIDCredential);
                });
            
            PInvoke.AppleAuth_IOS_LoginWithAppleId(requestId, (int)loginOptions);
        }
        
        public void GetCredentialState(
            string userId,
            Action<CredentialState> successCallback,
            Action<IAppleError> errorCallback)
        {
            var requestId = NativeMessageHandler.AddMessageCallback(
                this._scheduler,
                payload =>
                {
                    var response = this._payloadDeserializer.DeserializeCredentialStateResponse(payload);
                    if (response.Error != null)
                        errorCallback(response.Error);
                    else
                        successCallback(response.CredentialState);
                });
            
            PInvoke.AppleAuth_IOS_GetCredentialState(requestId, userId);
        }
        
        private static class PInvoke
        {
            [DllImport("__Internal")]
            public static extern void AppleAuth_IOS_GetCredentialState(uint requestId, string userId);

            [DllImport("__Internal")]
            public static extern void AppleAuth_IOS_LoginWithAppleId(uint requestId, int loginOptions);
            
            [DllImport("__Internal")]
            public static extern void AppleAuth_IOS_LoginSilently(uint requestId);
        }
    }
}