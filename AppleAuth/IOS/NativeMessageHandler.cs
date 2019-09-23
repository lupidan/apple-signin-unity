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
        
        private const uint InitialCallbackId = 1U;
        private const uint MaxCallbackId = uint.MaxValue;
        
        private static readonly Dictionary<uint, MessageCallbackEntry> CallbackDictionary = new Dictionary<uint, MessageCallbackEntry>();
        
        private static uint _callbackId = InitialCallbackId;
        private static bool _initialized = false;

        internal static uint AddMessageCallback(IMessageHandlerScheduler scheduler, bool isSingleUse, Action<string> messageCallback)
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
            if (_callbackId >= MaxCallbackId)
                _callbackId = InitialCallbackId;

            var callbackEntry = new MessageCallbackEntry(messageCallback, scheduler, isSingleUse);
            CallbackDictionary.Add(usedCallbackId, callbackEntry);
            return usedCallbackId;
        }
        
        internal static void RemoveMessageCallback(uint requestId)
        {
            if (!CallbackDictionary.ContainsKey(requestId))
                throw new Exception("Callback with id " + requestId + " does not exist and can't be removed");
            
            CallbackDictionary.Remove(requestId);
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
                    if (callbackEntry.IsSingleUseCallback)
                        CallbackDictionary.Remove(requestId);    
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("[NativeMessageHandler] Received exception while executing callback for request ID " + requestId);
                Console.WriteLine("[NativeMessageHandler] Exception: " + exception);
                Console.WriteLine("[NativeMessageHandler] Detailed payload:\n" + messagePayload);
            }
        }
        
        private class MessageCallbackEntry
        {
            public readonly Action<string> MessageCallback;
            public readonly IMessageHandlerScheduler Scheduler;
            public readonly bool IsSingleUseCallback;

            public MessageCallbackEntry(Action<string> messageCallback, IMessageHandlerScheduler scheduler, bool isSingleUseCallback)
            {
                this.MessageCallback = messageCallback;
                this.Scheduler = scheduler;
                this.IsSingleUseCallback = isSingleUseCallback;
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
