using System;
using AppleAuth.IOS.Interfaces;
using UnityEngine;

namespace AppleAuth.IOS.NativeMessages
{
    [Serializable]
    public class LoginWithAppleIdResponse : ILoginWithAppleIdResponse, ISerializationCallbackReceiver
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
        
        public void OnBeforeSerialize()
        {
            this._error = null;
            this._appleIdCredential = null;
            this._passwordCredential = null;
        }

        public void OnAfterDeserialize()
        {
            if (!this._hasError)
                this._error = null;

            if (!this._hasAppleIdCredential)
                this._appleIdCredential = null;

            if (!this._hasPasswordCredential)
                this._passwordCredential = null;
        }
    }
}
