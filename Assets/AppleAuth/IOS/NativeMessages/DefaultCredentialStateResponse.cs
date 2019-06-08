using System;
using AppleAuth.IOS.Interfaces;

namespace AppleAuth.IOS.NativeMessages
{
    [Serializable]
    public class DefaultCredentialStateResponse : ICredentialStateResponse
    {
        public CredentialState _credentialState;
        public DefaultAppleError _error;
        
        public CredentialState CredentialState { get { return this._credentialState; } }
        public IAppleError Error { get { return this._error; } }
    }
}