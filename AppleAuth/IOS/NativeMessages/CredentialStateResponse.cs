using System;
using AppleAuth.IOS.Enums;
using AppleAuth.IOS.Interfaces;
using UnityEngine;

namespace AppleAuth.IOS.NativeMessages
{
    [Serializable]
    public class CredentialStateResponse : ICredentialStateResponse, ISerializationCallbackReceiver
    {
        public bool _success;
        public bool _hasCredentialState;
        public bool _hasError;
        public CredentialState _credentialState;
        public AppleError _error;

        public bool Success { get { return this._success; } }
        public CredentialState CredentialState { get { return this._credentialState; } }
        public IAppleError Error { get { return this._error; } }
        
        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            SerializationTools.FixSerializationForObject(ref this._credentialState, this._hasCredentialState);
            SerializationTools.FixSerializationForObject(ref this._error, this._hasError);
        }
    }
}