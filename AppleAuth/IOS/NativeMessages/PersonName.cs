using AppleAuth.Interfaces;
using UnityEngine;

namespace AppleAuth.IOS.NativeMessages
{
    public class PersonName : IPersonName, ISerializationCallbackReceiver
    {
        public string _namePrefix;
        public string _givenName;
        public string _middleName;
        public string _familyName;
        public string _nameSuffix;
        public string _nickname;
        
        public string NamePrefix { get { return _namePrefix; } }
        public string GivenName { get { return _givenName; } }
        public string MiddleName { get { return _middleName; } }
        public string FamilyName { get { return _familyName; } }
        public string NameSuffix { get { return _nameSuffix; } }
        public string Nickname { get { return _nickname; } }
        public IPersonName PhoneticRepresentation { get { return null; } }
        
        public void OnBeforeSerialize() { }

        public virtual void OnAfterDeserialize()
        {
            SerializationTools.FixSerializationForString(ref this._namePrefix);
            SerializationTools.FixSerializationForString(ref this._givenName);
            SerializationTools.FixSerializationForString(ref this._middleName);
            SerializationTools.FixSerializationForString(ref this._familyName);
            SerializationTools.FixSerializationForString(ref this._nameSuffix);
            SerializationTools.FixSerializationForString(ref this._nickname);
        }
    }
}
