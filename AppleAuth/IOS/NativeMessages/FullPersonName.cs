using System;
using AppleAuth.IOS.Interfaces;

namespace AppleAuth.IOS.NativeMessages
{
    [Serializable]
    public class FullPersonName : PersonName, IPersonName
    {
        public bool _hasPhoneticRepresentation;
        public PersonName _phoneticRepresentation;

        public new IPersonName PhoneticRepresentation { get { return _phoneticRepresentation; } }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            
            SerializationTools.FixSerializationForObject(ref this._phoneticRepresentation, this._hasPhoneticRepresentation);
        }
    }
}
