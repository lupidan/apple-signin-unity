using System;
using AppleAuth.IOS.Enums;
using AppleAuth.IOS.Interfaces;

namespace AppleAuth.IOS.NativeMessages
{
    [Serializable]
    public class DefaultCredentialStateResponse : ICredentialStateResponse
    {
        public bool _success;
        public CredentialState _credentialState;
        public DefaultAppleError _error;

        public bool Success { get { return this._success; } }
        public CredentialState CredentialState { get { return this._credentialState; } }
        public IAppleError Error { get { return this._error; } }
    }
}