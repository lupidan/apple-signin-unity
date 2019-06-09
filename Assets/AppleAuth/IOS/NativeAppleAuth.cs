using System;
using System.Runtime.InteropServices;
using AppleAuth.IOS.Enums;
using AppleAuth.IOS.Interfaces;

namespace AppleAuth.IOS
{
    public class NativeAppleAuth
    {
        private readonly IPayloadDeserializer _payloadDeserializer;

        public NativeAppleAuth(IPayloadDeserializer payloadDeserializer)
        {
            this._payloadDeserializer = payloadDeserializer;
        }
        
        public void LoginSilently(
            Action<IAppleIDCredential> successCallback,
            Action<IAppleError> errorCallback)
        {
            var requestId = NativeMessageHandler.AddMessageCallback(payload =>
            {
                var response = this._payloadDeserializer.DeserializeLoginWithAppleIdResponse(payload);
                if (response.Error != null)
                    errorCallback(response.Error);
                else
                    successCallback(response.AppleIDCredential);
            });
            
            PInvoke.AppleAuth_IOS_LoginSilently(requestId);
        }
        
        public void LoginWithAppleId(
            Action<IAppleIDCredential> successCallback,
            Action<IAppleError> errorCallback)
        {
            var requestId = NativeMessageHandler.AddMessageCallback(payload =>
            {
                var response = this._payloadDeserializer.DeserializeLoginWithAppleIdResponse(payload);
                if (response.Error != null)
                    errorCallback(response.Error);
                else
                    successCallback(response.AppleIDCredential);
            });
            
            PInvoke.AppleAuth_IOS_LoginWithAppleId(requestId);
        }
        
        public void GetCredentialState(
            string userId,
            Action<CredentialState> successCallback,
            Action<IAppleError> errorCallback)
        {
            var requestId = NativeMessageHandler.AddMessageCallback(payload =>
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
            public static extern void AppleAuth_IOS_LoginWithAppleId(uint requestId);
            
            [DllImport("__Internal")]
            public static extern void AppleAuth_IOS_LoginSilently(uint requestId);
        }
    }
}