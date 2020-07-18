using AppleAuth.Interfaces;
using System;
using UnityEngine;

namespace AppleAuth.Native
{
    [Serializable]
    internal class LoginWithAppleIdResponse : ILoginWithAppleIdResponse, ISerializationCallbackReceiver
    {
        public bool _success;
        public bool _hasAppleIdCredential;
        public bool _hasPasswordCredential;
        public bool _hasError;
        public AppleIDCredential _appleIdCredential;
        public PasswordCredential _passwordCredential;
        public AppleError _error;

        public bool Success { get { return this._success; } }
        public IAppleError Error { get { return this._error; } }
        public IAppleIDCredential AppleIDCredential { get { return this._appleIdCredential; } }
        public IPasswordCredential PasswordCredential { get { return this._passwordCredential; } }
        
        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            SerializationTools.FixSerializationForObject(ref this._error, this._hasError);
            SerializationTools.FixSerializationForObject(ref this._appleIdCredential, this._hasAppleIdCredential);
            SerializationTools.FixSerializationForObject(ref this._passwordCredential, this._hasPasswordCredential);
        }
    }
}
