using System.Runtime.InteropServices;
using System.Text;
using AppleAuth.IOS.Enums;
using AppleAuth.IOS.Interfaces;

namespace AppleAuth.IOS.Extensions
{
    public static class PersonNameExtensions
    {
        private const string StringDictionaryFormat = "\"{0}\": \"{1}\",";
        private const string StringObjectFormat = "\"{0}\": {1},";
        
        public static string ToLocalizedString(this IPersonName personName, PersonNameFormatterStyle style, bool usePhoneticRepresentation = false)
        {
            var jsonString = JsonStringForPersonName(personName);
            return PInvoke.AppleAuth_IOS_GetPersonNameUsingFormatter(jsonString, (int) style, usePhoneticRepresentation);
        }

        private static string JsonStringForPersonName(IPersonName personName)
        {
            if (personName == null)
                return null;

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("{");
            
            TryAddKeyValue(StringDictionaryFormat, "_namePrefix", personName.NamePrefix, stringBuilder);
            TryAddKeyValue(StringDictionaryFormat, "_givenName", personName.GivenName, stringBuilder);
            TryAddKeyValue(StringDictionaryFormat, "_middleName", personName.MiddleName, stringBuilder);
            TryAddKeyValue(StringDictionaryFormat, "_familyName", personName.FamilyName, stringBuilder);
            TryAddKeyValue(StringDictionaryFormat, "_nameSuffix", personName.NameSuffix, stringBuilder);
            TryAddKeyValue(StringDictionaryFormat, "_nickname", personName.Nickname, stringBuilder);

            var phoneticRepresentationJson = JsonStringForPersonName(personName.PhoneticRepresentation);
            TryAddKeyValue(StringObjectFormat, "_phoneticRepresentation", phoneticRepresentationJson, stringBuilder);
            
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }

        private static void TryAddKeyValue(string format, string key, string value, StringBuilder stringBuilder)
        {
            if (string.IsNullOrEmpty(value))
                return;

            stringBuilder.AppendFormat(format, key, value);
        }
        
        private static class PInvoke
        {
            [DllImport("__Internal")]
            public static extern string AppleAuth_IOS_GetPersonNameUsingFormatter(string payload, int style, bool usePhoneticRepresentation);
        }
    }
}
