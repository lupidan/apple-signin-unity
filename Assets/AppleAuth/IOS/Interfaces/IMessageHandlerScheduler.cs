using System;

namespace AppleAuth.IOS.Interfaces
{
    public interface IMessageHandlerScheduler
    {
        void Schedule(Action callback);
        void Update();
    }
}
