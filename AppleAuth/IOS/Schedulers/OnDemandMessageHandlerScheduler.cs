using System;
using System.Collections.Generic;
using AppleAuth.IOS.Interfaces;

namespace AppleAuth.IOS
{
    public class OnDemandMessageHandlerScheduler : IMessageHandlerScheduler
    {
        private readonly List<Action> _pendingActions = new List<Action>();
        private readonly object _sync = new object();
        
        public void Schedule(Action callback)
        {
            if (callback == null)
                throw new Exception("Can't schedule a null callback");
            
            lock (_sync)
            {
                this._pendingActions.Add(callback);    
            }
        }

        public void Update()
        {
            int numberOfPendingActions;
            lock (_sync)
            {
                numberOfPendingActions = this._pendingActions.Count;
            }
            
            if (numberOfPendingActions == 0)
                return;

            Action[] actionsToExecute;
            lock (_sync)
            {
                actionsToExecute = _pendingActions.ToArray();
            }

            for (var i = 0; i < actionsToExecute.Length; i++)
            {
                var action = actionsToExecute[i];
                lock (_sync)
                {
                    _pendingActions.Remove(action);
                }

                action.Invoke();
            }
        }
    }
    
}