using AppleAuth.Interfaces;
using System;
using UnityEngine;

namespace AppleAuth.IOS.NativeMessages
{
    [Serializable]
    public class AppleError : IAppleError, ISerializationCallbackReceiver
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
        
        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            SerializationTools.FixSerializationForString(ref this._domain);
            SerializationTools.FixSerializationForString(ref this._localizedDescription);
            SerializationTools.FixSerializationForString(ref this._localizedRecoverySuggestion);
            SerializationTools.FixSerializationForString(ref this._localizedFailureReason);
            
            SerializationTools.FixSerializationForArray(ref this._localizedRecoveryOptions);
        }
    }
}