using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;

namespace AppleAuth.IOS
{
    internal static class NativeMessageHandler
    {
        private delegate void MessageHandlerInternalDelegate(uint requestId, string messagePayload);

        private static readonly Dictionary<uint, Action<string>> CallbackIndex = new Dictionary<uint, Action<string>>();
        private static uint _callbackId = 1;

        public static void Initialize()
        {
            PInvoke.SetupMessageHandlerInternalCallback(MessageHandlerInternalCallback);
        }

        public static uint AddMessageCallback(Action<string> callback)
        {
            var usedCallbackId = _callbackId;
            if (CallbackIndex.ContainsKey(usedCallbackId))
                throw new Exception("A Message index with same id already exists");

            CallbackIndex.Add(usedCallbackId, callback);
            _callbackId += 1;
            return usedCallbackId;
        }

        [MonoPInvokeCallback(typeof(MessageHandlerInternalDelegate))]
        private static void MessageHandlerInternalCallback(uint requestId, string messagePayload)
        {
            Action<string> callback;
            if (!CallbackIndex.TryGetValue(requestId, out callback))
                throw new Exception("Unhandled message with id " + requestId);

            callback.Invoke(messagePayload);
            CallbackIndex.Remove(requestId);
        }
        
        private static class PInvoke
        {
            [DllImport("__Internal")]
            public static extern void SetupMessageHandlerInternalCallback(MessageHandlerInternalDelegate internalCallback);
        }
    }
}
