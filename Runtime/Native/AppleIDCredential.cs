using AppleAuth.Enums;
using AppleAuth.Interfaces;
using System;
using UnityEngine;

namespace AppleAuth.Native
{
    [Serializable]
    internal class AppleIDCredential : IAppleIDCredential, ISerializationCallbackReceiver
    {
        public string _base64IdentityToken = null;
        public string _base64AuthorizationCode = null;
        public string _state = null;
        public string _user = null;
        public string[] _authorizedScopes = null;
        public bool _hasFullName = false;
        public FullPersonName _fullName = null;
        public string _email = null;
        public int _realUserStatus = 0;

        private byte[] _identityToken;
        private byte[] _authorizationCode;

        public byte[] IdentityToken { get { return this._identityToken; } }
        public byte[] AuthorizationCode { get { return this._authorizationCode; } }
        public string State { get { return this._state; } }
        public string User { get { return this._user; } }
        public string[] AuthorizedScopes { get { return this._authorizedScopes; } }
        public IPersonName FullName { get { return this._fullName; } }
        public string Email { get { return this._email; } }
        public RealUserStatus RealUserStatus { get { return (RealUserStatus) this._realUserStatus; } }
        
        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            SerializationTools.FixSerializationForString(ref this._base64IdentityToken);
            SerializationTools.FixSerializationForString(ref this._base64AuthorizationCode);
            SerializationTools.FixSerializationForString(ref this._state);
            SerializationTools.FixSerializationForString(ref this._user);
            SerializationTools.FixSerializationForString(ref this._email);
            
            SerializationTools.FixSerializationForArray(ref this._authorizedScopes);
            
            SerializationTools.FixSerializationForObject(ref this._fullName, this._hasFullName);

            this._identityToken = SerializationTools.GetBytesFromBase64String(this._base64IdentityToken, "_identityToken");
            this._authorizationCode = SerializationTools.GetBytesFromBase64String(this._base64AuthorizationCode, "_authorizationCode");
        }
    }
}
