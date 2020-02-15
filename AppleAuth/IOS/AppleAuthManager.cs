using AppleAuth.Enums;
using AppleAuth.Interfaces;
using System;
using System.Collections.Generic;

namespace AppleAuth.IOS
{
    public class AppleAuthManager : IAppleAuthManager
    {
        private readonly IPayloadDeserializer _payloadDeserializer;

        private uint _registeredCredentialsRevokedCallbackId = 0U;

        public bool IsCurrentPlatformSupported
        {
            get
            {
                return PInvoke.AppleAuth_IOS_IsCurrentPlatformSupported();
            }
        }

        public AppleAuthManager(IPayloadDeserializer payloadDeserializer)
        {
            this._payloadDeserializer = payloadDeserializer;
        }
        
        public void QuickLogin(
            AppleAuthQuickLoginArgs quickLoginArgs,
            Action<ICredential> successCallback,
            Action<IAppleError> errorCallback)
        {
            var nonce = quickLoginArgs.Nonce;
            var requestId = CallbackHandler.AddMessageCallback(
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

            PInvoke.AppleAuth_IOS_QuickLogin(requestId, nonce);
        }
        
        public void LoginWithAppleId(
            AppleAuthLoginArgs loginArgs,
            Action<ICredential> successCallback,
            Action<IAppleError> errorCallback)
        {
            var loginOptions = loginArgs.Options;
            var nonce = loginArgs.Nonce;
            var requestId = CallbackHandler.AddMessageCallback(
                true,
                payload =>
                {
                    var response = this._payloadDeserializer.DeserializeLoginWithAppleIdResponse(payload);
                    if (response.Error != null)
                        errorCallback(response.Error);
                    else
                        successCallback(response.AppleIDCredential);
                });
            
            PInvoke.AppleAuth_IOS_LoginWithAppleId(requestId, (int)loginOptions, nonce);
        }
        
        public void GetCredentialState(
            string userId,
            Action<CredentialState> successCallback,
            Action<IAppleError> errorCallback)
        {
            var requestId = CallbackHandler.AddMessageCallback(
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
        }
        
        public void SetCredentialsRevokedCallback(Action<string> credentialsRevokedCallback)
        {
            if (this._registeredCredentialsRevokedCallbackId != 0)
            {
                CallbackHandler.RemoveMessageCallback(this._registeredCredentialsRevokedCallbackId);
                this._registeredCredentialsRevokedCallbackId = 0;
            }

            if (credentialsRevokedCallback != null)
            {
                this._registeredCredentialsRevokedCallbackId = CallbackHandler.AddMessageCallback(
                    false,
                    credentialsRevokedCallback);
            }
            
            PInvoke.AppleAuth_IOS_RegisterCredentialsRevokedCallbackId(this._registeredCredentialsRevokedCallbackId);
        }

        public void Update()
        {
            CallbackHandler.ExecutePendingCallbacks();
        }

        private static class CallbackHandler
        {
            private const uint InitialCallbackId = 1U;
            private const uint MaxCallbackId = uint.MaxValue;

            private static readonly object SyncLock = new object();
            private static readonly Dictionary<uint, Entry> CallbackDictionary = new Dictionary<uint, Entry>();
            private static readonly List<Action> ScheduledActions = new List<Action>();

            private static uint _callbackId = InitialCallbackId;
            private static bool _initialized = false;

            public static void ScheduleCallback(uint requestId, string payload)
            {
                lock (SyncLock)
                {
                    var callbackEntry = default(Entry);
                    if (CallbackDictionary.TryGetValue(requestId, out callbackEntry))
                    {
                        var callback = callbackEntry.MessageCallback;
                        ScheduledActions.Add(() => callback.Invoke(payload));

                        if (callbackEntry.IsSingleUseCallback)
                        {
                            CallbackDictionary.Remove(requestId);
                        }
                    }
                }
            }

            public static void ExecutePendingCallbacks()
            {
                lock (SyncLock)
                {
                    while (ScheduledActions.Count > 0)
                    {
                        var action = ScheduledActions[0];
                        ScheduledActions.RemoveAt(0);
                        action.Invoke();
                    }
                }
            }

            public static uint AddMessageCallback(bool isSingleUse, Action<string> messageCallback)
            {
                if (!_initialized)
                {
                    PInvoke.AppleAuth_IOS_SetupNativeMessageHandlerCallback(PInvoke.NativeMessageHandlerCallback);
                    _initialized = true;
                }

                if (messageCallback == null)
                {
                    throw new Exception("Can't add a null Message Callback.");
                }

                var usedCallbackId = default(uint);
                lock (SyncLock)
                {
                    usedCallbackId = _callbackId;
                    _callbackId += 1;
                    if (_callbackId >= MaxCallbackId)
                        _callbackId = InitialCallbackId;

                    var callbackEntry = new Entry(isSingleUse, messageCallback);
                    CallbackDictionary.Add(usedCallbackId, callbackEntry);
                }
                return usedCallbackId;
            }

            public static void RemoveMessageCallback(uint requestId)
            {
                lock (SyncLock)
                {
                    if (!CallbackDictionary.ContainsKey(requestId))
                    {
                        throw new Exception("Callback with id " + requestId + " does not exist and can't be removed");
                    }

                    CallbackDictionary.Remove(requestId);
                }
            }

            private class Entry
            {
                public readonly bool IsSingleUseCallback;
                public readonly Action<string> MessageCallback;

                public Entry(bool isSingleUseCallback, Action<string> messageCallback)
                {
                    this.IsSingleUseCallback = isSingleUseCallback;
                    this.MessageCallback = messageCallback;
                }
            }
        }

        private static class PInvoke
        {
            public delegate void NativeMessageHandlerCallbackDelegate(uint requestId, string payload);

            [AOT.MonoPInvokeCallback(typeof(NativeMessageHandlerCallbackDelegate))]
            public static void NativeMessageHandlerCallback(uint requestId, string payload)
            {
                try
                {
                    CallbackHandler.ScheduleCallback(requestId, payload);
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Received exception while scheduling a callback for request ID " + requestId);
                    Console.WriteLine("Detailed payload:\n" + payload);
                    Console.WriteLine("Exception: " + exception);
                }
            }

            [System.Runtime.InteropServices.DllImport("__Internal")]
            public static extern bool AppleAuth_IOS_IsCurrentPlatformSupported();

            [System.Runtime.InteropServices.DllImport("__Internal")]
            public static extern void AppleAuth_IOS_SetupNativeMessageHandlerCallback(NativeMessageHandlerCallbackDelegate callback);
            
            [System.Runtime.InteropServices.DllImport("__Internal")]
            public static extern void AppleAuth_IOS_GetCredentialState(uint requestId, string userId);

            [System.Runtime.InteropServices.DllImport("__Internal")]
            public static extern void AppleAuth_IOS_LoginWithAppleId(uint requestId, int loginOptions, string nonceCStr);
            
            [System.Runtime.InteropServices.DllImport("__Internal")]
            public static extern void AppleAuth_IOS_QuickLogin(uint requestId, string nonceCStr);
            
            [System.Runtime.InteropServices.DllImport("__Internal")]
            public static extern void AppleAuth_IOS_RegisterCredentialsRevokedCallbackId(uint callbackId);
        }
    }
}
