using System;
using AppleAuth.IOS.Interfaces;

namespace AppleAuth.IOS.NativeMessages
{
    [Serializable]
    public class DefaultAppleError : IAppleError
    {
        public int _code;
        public string _domain;
        public string _localizedDescription;
        public string[] _localizedRecoveryOptions;
        public string _localizedRecoverySuggestion;
        public string _localizedFailureReason;
        
        public int Code { get { return this._code; } }
        public string Domain { get { return this._domain; } }
        public string LocalizedDescription { get { return this._localizedDescription; } }
        public string[] LocalizedRecoveryOptions { get { return this._localizedRecoveryOptions; } }
        public string LocalizedRecoverySuggestion { get { return this._localizedRecoverySuggestion; } }
        public string LocalizedFailureReason { get { return this._localizedFailureReason; } }
    }
}