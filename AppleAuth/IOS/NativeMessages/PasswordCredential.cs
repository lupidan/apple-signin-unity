using AppleAuth.Interfaces;
using System;
using UnityEngine;

namespace AppleAuth.IOS.NativeMessages
{
    [Serializable]
    public class PasswordCredential : IPasswordCredential, ISerializationCallbackReceiver
    {
        public string _user;
        public string _password;
        
        public string User { get { return this._user; } }
        public string Password { get { return this._password; } }
        
        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            SerializationTools.FixSerializationForString(ref this._user);
            SerializationTools.FixSerializationForString(ref this._password);
        }
    }
}
