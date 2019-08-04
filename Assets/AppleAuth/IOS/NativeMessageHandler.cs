#if UNITY_IOS && !UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using AppleAuth.IOS.Interfaces;

namespace AppleAuth.IOS
{
    internal static class NativeMessageHandler
    {
        private delegate void NativeMessageHandlerDelegate(uint requestId, string messagePayload);

        private static readonly Dictionary<uint, MessageCallbackEntry> CallbackDictionary = new Dictionary<uint, MessageCallbackEntry>();
        private static uint _callbackId = 1;
        private static bool _initialized = false;

        internal static uint AddMessageCallback(IMessageHandlerScheduler scheduler, Action<string> messageCallback)
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

            var callbackEntry = new MessageCallbackEntry(messageCallback, scheduler);
            CallbackDictionary.Add(usedCallbackId, callbackEntry);
            return usedCallbackId;
        }

        [MonoPInvokeCallback(typeof(NativeMessageHandlerDelegate))]
        private static void NativeMessageHandlerCallback(uint requestId, string messagePayload)
        {
            try
            {
                MessageCallbackEntry callbackEntry;
                if (CallbackDictionary.TryGetValue(requestId, out callbackEntry))
                {
                    callbackEntry.Scheduler.Schedule(() => callbackEntry.MessageCallback.Invoke(messagePayload));
                    CallbackDictionary.Remove(requestId);    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[NativeMessageHandler] Received exception while dealing with callback message with request ID " + requestId);
                Console.WriteLine("[NativeMessageHandler] Detailed payload:\n" + messagePayload);
            }
        }
        
        private class MessageCallbackEntry
        {
            public readonly Action<string> MessageCallback;
            public readonly IMessageHandlerScheduler Scheduler;

            public MessageCallbackEntry(Action<string> messageCallback, IMessageHandlerScheduler scheduler)
            {
                this.MessageCallback = messageCallback;
                this.Scheduler = scheduler;
            }
        }
        
        private static class PInvoke
        {
            [DllImport("__Internal")]
            public static extern void AppleAuth_IOS_SetupNativeMessageHandlerCallback(NativeMessageHandlerDelegate callback);
        }
    }
}
#endif
