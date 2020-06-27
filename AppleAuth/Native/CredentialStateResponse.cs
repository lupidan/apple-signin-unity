using AppleAuth.Enums;
using AppleAuth.Interfaces;
using System;
using UnityEngine;

namespace AppleAuth.Native
{
    [Serializable]
    public class CredentialStateResponse : ICredentialStateResponse, ISerializationCallbackReceiver
    {
        public bool _success;
        public bool _hasCredentialState;
        public bool _hasError;
        public int _credentialStateRaw;
        public AppleError _error;

        private CredentialState _credentialState;

        public bool Success { get { return this._success; } }
        public CredentialState CredentialState { get { return this._credentialState; } }
        public IAppleError Error { get { return this._error; } }
        
        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            SerializationTools.FixSerializationForObject(ref this._credentialStateRaw, this._hasCredentialState);
            SerializationTools.FixSerializationForObject(ref this._error, this._hasError);

            this._credentialState = (CredentialState) this._credentialStateRaw;
        }
    }
}
