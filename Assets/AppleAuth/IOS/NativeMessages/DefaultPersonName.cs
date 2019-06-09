using System;
using AppleAuth.IOS.Interfaces;

namespace AppleAuth.IOS.NativeMessages
{
    [Serializable]
    public class DefaultPersonName : IPersonName
    {
        public string _namePrefix;
        public string _givenName;
        public string _middleName;
        public string _familyName;
        public string _nameSuffix;
        public string _nickname;
        public DefaultPersonName _phoneticRepresentation;
        
        public string NamePrefix { get { return _namePrefix; } }
        public string GivenName { get { return _givenName; } }
        public string MiddleName { get { return _middleName; } }
        public string FamilyName { get { return _familyName; } }
        public string NameSuffix { get { return _nameSuffix; } }
        public string Nickname { get { return _nickname; } }
        public IPersonName PhoneticRepresentation { get { return _phoneticRepresentation; } }
    }
}
