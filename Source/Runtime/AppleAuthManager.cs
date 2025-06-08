#if ((UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS || UNITY_STANDALONE_OSX) && !UNITY_EDITOR)
#define APPLE_AUTH_MANAGER_NATIVE_IMPLEMENTATION_AVAILABLE
#endif

using AppleAuth.Enums;
using AppleAuth.Interfaces;
using System;

namespace AppleAuth
{
    public class AppleAuthManager : IAppleAuthManager
    {
        static AppleAuthManager()
        {
            const string versionMessage = "Using Sign in with Apple Unity Plugin - 1.5.0";
#if APPLE_AUTH_MANAGER_NATIVE_IMPLEMENTATION_AVAILABLE
            PInvoke.AppleAuth_LogMessage(versionMessage);
#else
            UnityEngine.Debug.Log(versionMessage);
#endif
        }

#if APPLE_AUTH_MANAGER_NATIVE_IMPLEMENTATION_AVAILABLE
        private readonly IPayloadDeserializer _payloadDeserializer;

        private Action<string> _credentialsRevokedCallback;
#endif

        public static bool IsCurrentPlatformSupported
        {
            get
            {
#if APPLE_AUTH_MANAGER_NATIVE_IMPLEMENTATION_AVAILABLE
                return PInvoke.AppleAuth_IsCurrentPlatformSupported();
#else
                return false;
#endif
            }
        }

        public AppleAuthManager(IPayloadDeserializer payloadDeserializer)
        {
#if APPLE_AUTH_MANAGER_NATIVE_IMPLEMENTATION_AVAILABLE
            this._payloadDeserializer = payloadDeserializer;
#endif
        }

        public void QuickLogin(Action<ICredential> successCallback, Action<IAppleError> errorCallback)
        {
            this.QuickLogin(new AppleAuthQuickLoginArgs(), successCallback, errorCallback);
        }

        public void QuickLogin(
            AppleAuthQuickLoginArgs quickLoginArgs,
            Action<ICredential> successCallback,
            Action<IAppleError> errorCallback)
        {
#if APPLE_AUTH_MANAGER_NATIVE_IMPLEMENTATION_AVAILABLE
            var nonce = quickLoginArgs.Nonce;
            var state = quickLoginArgs.State;
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

            PInvoke.AppleAuth_QuickLogin(requestId, nonce, state);
#else
            throw new Exception("AppleAuthManager is not supported in this platform");
#endif
        }

        public void LoginWithAppleId(LoginOptions options, Action<ICredential> successCallback, Action<IAppleError> errorCallback)
        {
            this.LoginWithAppleId(new AppleAuthLoginArgs(options), successCallback, errorCallback);
        }

        public void LoginWithAppleId(
            AppleAuthLoginArgs loginArgs,
            Action<ICredential> successCallback,
            Action<IAppleError> errorCallback)
        {
#if APPLE_AUTH_MANAGER_NATIVE_IMPLEMENTATION_AVAILABLE
            var loginOptions = loginArgs.Options;
            var nonce = loginArgs.Nonce;
            var state = loginArgs.State;
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
            
            PInvoke.AppleAuth_LoginWithAppleId(requestId, (int)loginOptions, nonce, state);
#else
            throw new Exception("AppleAuthManager is not supported in this platform");
#endif
        }
        
        public void GetCredentialState(
            string userId,
            Action<CredentialState> successCallback,
            Action<IAppleError> errorCallback)
        {
#if APPLE_AUTH_MANAGER_NATIVE_IMPLEMENTATION_AVAILABLE
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
            
            PInvoke.AppleAuth_GetCredentialState(requestId, userId);
#else
            throw new Exception("AppleAuthManager is not supported in this platform");
#endif
        }
        
        public void SetCredentialsRevokedCallback(Action<string> credentialsRevokedCallback)
        {
#if APPLE_AUTH_MANAGER_NATIVE_IMPLEMENTATION_AVAILABLE
            if (this._credentialsRevokedCallback != null)
            {
                CallbackHandler.NativeCredentialsRevoked -= this._credentialsRevokedCallback;
                this._credentialsRevokedCallback = null;
            }

            if (credentialsRevokedCallback != null)
            {
                CallbackHandler.NativeCredentialsRevoked += credentialsRevokedCallback;
                this._credentialsRevokedCallback = credentialsRevokedCallback;
            }
#endif
        }

        public void Update()
        {
#if APPLE_AUTH_MANAGER_NATIVE_IMPLEMENTATION_AVAILABLE
            CallbackHandler.ExecutePendingCallbacks();
#endif
        }

#if APPLE_AUTH_MANAGER_NATIVE_IMPLEMENTATION_AVAILABLE
        private static class CallbackHandler
        {
            private const uint InitialCallbackId = 1U;
            private const uint MaxCallbackId = uint.MaxValue;

            private static readonly object SyncLock = new object();
            private static readonly System.Collections.Generic.Dictionary<uint, Entry> CallbackDictionary = new System.Collections.Generic.Dictionary<uint, Entry>();
            private static readonly System.Collections.Generic.List<Action> ScheduledActions = new System.Collections.Generic.List<Action>();

            private static uint _callbackId = InitialCallbackId;
            private static bool _initialized = false;
            
            private static uint _credentialsRevokedCallbackId = 0U;
            private static event Action<string> _nativeCredentialsRevoked = null;

            public static event Action<string> NativeCredentialsRevoked
            {
                add
                {
                    lock (SyncLock)
                    {
                        if (_nativeCredentialsRevoked == null)
                        {
                            _credentialsRevokedCallbackId = AddMessageCallback(false, payload => _nativeCredentialsRevoked.Invoke(payload));
                            PInvoke.AppleAuth_RegisterCredentialsRevokedCallbackId(_credentialsRevokedCallbackId);
                        }

                        _nativeCredentialsRevoked += value;
                    }
                }

                remove
                {
                    lock (SyncLock)
                    {
                        _nativeCredentialsRevoked -= value;

                        if (_nativeCredentialsRevoked == null)
                        {
                            RemoveMessageCallback(_credentialsRevokedCallbackId);
                            _credentialsRevokedCallbackId = 0U;
                            PInvoke.AppleAuth_RegisterCredentialsRevokedCallbackId(0U);
                        }
                    }
                }
            }

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
                    PInvoke.AppleAuth_SetupNativeMessageHandlerCallback(PInvoke.NativeMessageHandlerCallback);
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
#if UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS
            private const string DllName = "__Internal";
#elif UNITY_STANDALONE_OSX
            private const string DllName = "MacOSAppleAuthManager";
#endif

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

            [System.Runtime.InteropServices.DllImport(DllName)]
            public static extern bool AppleAuth_IsCurrentPlatformSupported();

            [System.Runtime.InteropServices.DllImport(DllName)]
            public static extern void AppleAuth_SetupNativeMessageHandlerCallback(NativeMessageHandlerCallbackDelegate callback);
            
            [System.Runtime.InteropServices.DllImport(DllName)]
            public static extern void AppleAuth_GetCredentialState(uint requestId, string userId);

            [System.Runtime.InteropServices.DllImport(DllName)]
            public static extern void AppleAuth_LoginWithAppleId(uint requestId, int loginOptions, string nonceCStr, string stateCStr);
            
            [System.Runtime.InteropServices.DllImport(DllName)]
            public static extern void AppleAuth_QuickLogin(uint requestId, string nonceCStr, string stateCStr);
            
            [System.Runtime.InteropServices.DllImport(DllName)]
            public static extern void AppleAuth_RegisterCredentialsRevokedCallbackId(uint callbackId);

            [System.Runtime.InteropServices.DllImport(DllName)]
            public static extern void AppleAuth_LogMessage(string messageCStr);
        }
#endif
    }
}
