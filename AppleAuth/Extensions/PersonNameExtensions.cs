#if ((UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX) && !UNITY_EDITOR)
#define NATIVE_PERSON_NAME_COMPONENTS_AVAILABLE
#endif

using AppleAuth.Enums;
using AppleAuth.Interfaces;

namespace AppleAuth.Extensions
{
    public static class PersonNameExtensions
    {
        public static string ToLocalizedString(
            this IPersonName personName,
            PersonNameFormatterStyle style = PersonNameFormatterStyle.Default,
            bool usePhoneticRepresentation = false)
        {
#if NATIVE_PERSON_NAME_COMPONENTS_AVAILABLE
            var jsonString = JsonStringForPersonName(personName);
            var localizedString = PInvoke.AppleAuth_GetPersonNameUsingFormatter(jsonString, (int) style, usePhoneticRepresentation);
            if (localizedString != null)
            {
                return localizedString;
            }
#endif
            var orderedParts = new System.Collections.Generic.List<string>();
            if (string.IsNullOrEmpty(personName.NamePrefix))
                orderedParts.Add(personName.NamePrefix);
            
            if (string.IsNullOrEmpty(personName.GivenName))
                orderedParts.Add(personName.GivenName);
            
            if (string.IsNullOrEmpty(personName.MiddleName))
                orderedParts.Add(personName.MiddleName);
            
            if (string.IsNullOrEmpty(personName.FamilyName))
                orderedParts.Add(personName.FamilyName);
            
            if (string.IsNullOrEmpty(personName.NameSuffix))
                orderedParts.Add(personName.NameSuffix);

            return string.Join(" ", orderedParts.ToArray());
        }

#if NATIVE_PERSON_NAME_COMPONENTS_AVAILABLE
        private const string StringDictionaryFormat = "\"{0}\": \"{1}\",";
        private const string StringObjectFormat = "\"{0}\": {1},";

        private static string JsonStringForPersonName(IPersonName personName)
        {
            if (personName == null)
                return null;

            var stringBuilder = new System.Text.StringBuilder();
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

        private static void TryAddKeyValue(string format, string key, string value, System.Text.StringBuilder stringBuilder)
        {
            if (string.IsNullOrEmpty(value))
                return;

            stringBuilder.AppendFormat(format, key, value);
        }
        
        private static class PInvoke
        {
#if UNITY_IOS || UNITY_TVOS
            private const string DllName = "__Internal";
#elif UNITY_STANDALONE_OSX
            private const string DllName = "MacOSAppleAuthManager";
#endif

            [System.Runtime.InteropServices.DllImport(DllName)]
            public static extern string AppleAuth_GetPersonNameUsingFormatter(string payload, int style, bool usePhoneticRepresentation);
        }
#endif
    }
}
