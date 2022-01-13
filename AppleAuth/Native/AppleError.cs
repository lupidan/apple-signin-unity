using AppleAuth.Interfaces;
using System;
using UnityEngine;

namespace AppleAuth.Native
{
    [Serializable]
    internal class AppleError : IAppleError, ISerializationCallbackReceiver
    {
        public int _code = 0;
        public string _domain = null;
        public string _localizedDescription = null;
        public string[] _localizedRecoveryOptions = null;
        public string _localizedRecoverySuggestion = null;
        public string _localizedFailureReason = null;
        
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

        public override string ToString()
        {
            return $"Domain={_domain} Code={_code} Description={_localizedDescription}";
        }
    }
}