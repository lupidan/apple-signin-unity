using System;
using AppleAuth.Interfaces;
using UnityEngine;

namespace AppleAuth.Native
{
    [Serializable]
    internal class PersonName : IPersonName, ISerializationCallbackReceiver
    {
        public string _namePrefix = null;
        public string _givenName = null;
        public string _middleName = null;
        public string _familyName = null;
        public string _nameSuffix = null;
        public string _nickname = null;
        
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
