using System;
using AppleAuth.IOS.Interfaces;

namespace AppleAuth.IOS.NativeMessages
{
    [Serializable]
    public class DefaultLoginWithAppleIdResponse : ILoginWithAppleIdResponse
    {
        public bool _success;
        public DefaultAppleError _error;
        
        public bool Success { get { return this._success; } }
        public IAppleError Error { get { return this._error; } }
    }
}
