using AppleAuth.Enums;
using AppleAuth.Interfaces;
using System;
using UnityEngine;

namespace AppleAuth.Native
{
    [Serializable]
    public class AppleIDCredential : IAppleIDCredential, ISerializationCallbackReceiver
    {
        public string _base64IdentityToken;
        public string _base64AuthorizationCode;
        public string _state;
        public string _user;
        public string[] _authorizedScopes;
        public bool _hasFullName;
        public FullPersonName _fullName;
        public string _email;
        public int _realUserStatusRaw;

        private RealUserStatus _realUserStatus;
        private byte[] _identityToken;
        private byte[] _authorizationCode;

        public byte[] IdentityToken { get { return this._identityToken; } }
        public byte[] AuthorizationCode { get { return this._authorizationCode; } }
        public string State { get { return this._state; } }
        public string User { get { return this._user; } }
        public string[] AuthorizedScopes { get { return this._authorizedScopes; } }
        public IPersonName FullName { get { return this._fullName; } }
        public string Email { get { return this._email; } }
        public RealUserStatus RealUserStatus { get { return this._realUserStatus; } }
        
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
            
            SerializationTools.FixSerializationForFullPersonName(ref this._fullName);

            this._identityToken = SerializationTools.GetBytesFromBase64String(this._base64IdentityToken, "_identityToken");
            this._authorizationCode = SerializationTools.GetBytesFromBase64String(this._base64AuthorizationCode, "_authorizationCode");
            this._realUserStatus = (RealUserStatus) this._realUserStatusRaw;
        }
    }
}
