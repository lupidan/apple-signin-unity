using System;
using AppleAuth.IOS.Interfaces;
using UnityEngine;

namespace AppleAuth.IOS.NativeMessages
{
    [Serializable]
    public class DefaultPersonName : DefaultPersonNameBase, IPersonName, ISerializationCallbackReceiver
    {
        public bool _hasPhoneticRepresentation;
        public DefaultPersonNameBase _phoneticRepresentation;

        public new IPersonName PhoneticRepresentation { get { return _phoneticRepresentation; } }
        
        public void OnBeforeSerialize()
        {
            this._phoneticRepresentation = null;
        }

        public void OnAfterDeserialize()
        {
            if (!this._hasPhoneticRepresentation)
                this._phoneticRepresentation = null;
        }
    }
}
