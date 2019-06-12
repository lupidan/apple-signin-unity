using System;
using AppleAuth.IOS.Interfaces;

namespace AppleAuth.IOS
{
    public class ImmediateMessageHandlerScheduler : IMessageHandlerScheduler
    {
        public void Schedule(Action callback)
        {
            if (callback == null)
                throw new Exception("Can't schedule a null callback");
            
            callback.Invoke();
        }

        public void Update()
        {
        }
    }
}
