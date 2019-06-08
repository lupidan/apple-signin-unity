using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;

namespace AppleAuth.IOS
{
    internal static class NativeMessageHandler
    {
        private delegate void NativeMessageHandlerDelegate(uint requestId, string messagePayload);

        private static readonly Dictionary<uint, Action<string>> CallbackDictionary = new Dictionary<uint, Action<string>>();
        private static uint _callbackId = 1;
        private static bool _initialized = false;

        public static uint AddMessageCallback(Action<string> messageCallback)
        {
            if (!_initialized)
            {
                PInvoke.AppleAuth_IOS_SetupNativeMessageHandlerCallback(NativeMessageHandlerCallback);
                _initialized = true;
            }

            if (messageCallback == null)
                throw new Exception("Can't add a null Message Callback.");
            
            var usedCallbackId = _callbackId;
            _callbackId += 1;
            if (CallbackDictionary.ContainsKey(usedCallbackId))
                throw new Exception("A Message Callback with the same ID " + usedCallbackId + " already exists.");

            CallbackDictionary.Add(usedCallbackId, messageCallback);
            return usedCallbackId;
        }

        [MonoPInvokeCallback(typeof(NativeMessageHandlerDelegate))]
        private static void NativeMessageHandlerCallback(uint requestId, string messagePayload)
        {
            Action<string> callback;
            if (!CallbackDictionary.TryGetValue(requestId, out callback))
                throw new Exception("A Message Callback with ID " + requestId + " couldn't be found");

            callback.Invoke(messagePayload);
            CallbackDictionary.Remove(requestId);
        }
        
        private static class PInvoke
        {
            [DllImport("__Internal")]
            public static extern void AppleAuth_IOS_SetupNativeMessageHandlerCallback(NativeMessageHandlerDelegate callback);
        }
    }
}
