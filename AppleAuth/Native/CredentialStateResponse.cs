using AppleAuth.Enums;
using AppleAuth.Interfaces;
using System;
using UnityEngine;

namespace AppleAuth.Native
{
    [Serializable]
    internal class CredentialStateResponse : ICredentialStateResponse, ISerializationCallbackReceiver
    {
        public bool _success = false;
        public bool _hasCredentialState = false;
        public bool _hasError = false;
        public int _credentialState = 0;
        public AppleError _error = null;

        public bool Success { get { return this._success; } }
        public CredentialState CredentialState { get { return (CredentialState) this._credentialState; } }
        public IAppleError Error { get { return this._error; } }
        
        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            SerializationTools.FixSerializationForObject(ref this._credentialState, this._hasCredentialState);
            SerializationTools.FixSerializationForObject(ref this._error, this._hasError);
        }
    }
}
