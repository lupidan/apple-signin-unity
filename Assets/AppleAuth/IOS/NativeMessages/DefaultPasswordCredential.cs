using System;
using AppleAuth.IOS.Interfaces;

namespace AppleAuth.IOS.NativeMessages
{
    [Serializable]
    public class DefaultPasswordCredential : IPasswordCredential
    {
        public string _user;
        public string _password;
        
        public string User { get { return this._user; } }
        public string Password { get { return this._password; } }        
    }
}
