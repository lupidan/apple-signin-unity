using System;
using AppleAuth.IOS.Enums;
using AppleAuth.IOS.Interfaces;
using UnityEngine;

namespace AppleAuth.IOS.NativeMessages
{
    [Serializable]
    public class DefaultAppleIDCredential : IAppleIDCredential, ISerializationCallbackReceiver
    {
        public string _identityToken;
        public string _authorizationCode;
        public string _state;
        public string _user;
        public string[] _authorizedScopes;
        public bool _hasFullName;
        public DefaultPersonName _fullName;
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
        
        public void OnBeforeSerialize()
        {
            this._fullName = null;
        }

        public void OnAfterDeserialize()
        {
            if (!this._hasFullName)
                this._fullName = null;
        }
    }
}
