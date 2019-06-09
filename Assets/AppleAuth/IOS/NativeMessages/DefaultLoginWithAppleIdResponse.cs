using System;
using AppleAuth.IOS.Interfaces;

namespace AppleAuth.IOS.NativeMessages
{
    [Serializable]
    public class DefaultLoginWithAppleIdResponse : ILoginWithAppleIdResponse
    {
        public bool _success;
        public DefaultAppleError _error;
        public DefaultAppleIDCredential _appleIdCredential;
        public DefaultPasswordCredential _passwordCredential;
        
        public bool Success { get { return this._success; } }
        public IAppleError Error { get { return this._error; } }
        public IAppleIDCredential AppleIDCredential { get { return this._appleIdCredential; } }
        public IPasswordCredential PasswordCredential { get { return this._passwordCredential; } }
    }
}
