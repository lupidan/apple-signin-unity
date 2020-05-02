using AppleAuth.Enums;
using AppleAuth.Interfaces;
using System;
using UnityEngine;

namespace AppleAuth.Native
{
    [Serializable]
    public class AppleIDCredential : IAppleIDCredential, ISerializationCallbackReceiver
    {
        public string _identityToken;
        public string _authorizationCode;
        public string _state;
        public string _user;
        public string[] _authorizedScopes;
        public bool _hasFullName;
        public FullPersonName _fullName;
        public string _email;
        public RealUserStatus _realUserStatus;
        
        public byte[] IdentityToken { get { return Convert.FromBase64String(this._identityToken); } }
        public byte[] AuthorizationCode { get { return Convert.FromBase64String(this._authorizationCode); } }
        public string State { get { return this._state; } }
        public string User { get { return this._user; } }
        public string[] AuthorizedScopes { get { return this._authorizedScopes; } }
        public IPersonName FullName { get { return this._fullName; } }
        public string Email { get { return this._email; } }
        public RealUserStatus RealUserStatus { get { return this._realUserStatus; } }
        
        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            SerializationTools.FixSerializationForString(ref this._identityToken);
            SerializationTools.FixSerializationForString(ref this._authorizationCode);
            SerializationTools.FixSerializationForString(ref this._state);
            SerializationTools.FixSerializationForString(ref this._user);
            SerializationTools.FixSerializationForString(ref this._email);
            
            SerializationTools.FixSerializationForArray(ref this._authorizedScopes);
            
            SerializationTools.FixSerializationForObject(ref this._fullName, this._hasFullName);
            
            SerializationTools.FixSerializationForFullPersonName(ref this._fullName);
        }
    }
}
