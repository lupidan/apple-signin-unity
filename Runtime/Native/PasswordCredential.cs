using AppleAuth.Interfaces;
using System;
using UnityEngine;

namespace AppleAuth.Native
{
    [Serializable]
    internal class PasswordCredential : IPasswordCredential, ISerializationCallbackReceiver
    {
        public string _user = null;
        public string _password = null;
        
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
