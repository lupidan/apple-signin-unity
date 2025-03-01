using AppleAuth.Interfaces;
using System;

namespace AppleAuth.Native
{
    [Serializable]
    internal class FullPersonName : PersonName, IPersonName
    {
        public bool _hasPhoneticRepresentation = false;
        public PersonName _phoneticRepresentation = null;

        public new IPersonName PhoneticRepresentation { get { return _phoneticRepresentation; } }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            
            SerializationTools.FixSerializationForObject(ref this._phoneticRepresentation, this._hasPhoneticRepresentation);
        }
    }
}
