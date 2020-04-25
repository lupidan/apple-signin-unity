namespace AppleAuth.Native
{
    internal static class SerializationTools
    {
        internal static void FixSerializationForString(ref string originalString)
        {
            if (string.IsNullOrEmpty(originalString))
                originalString = null;
        }

        internal static void FixSerializationForArray<T>(ref T[] originalArray)
        {
            if (originalArray != null && originalArray.Length == 0)
                originalArray = null;
        }

        internal static void FixSerializationForObject<T>(ref T originalObject, bool hasOriginalObject)
        {
            if (!hasOriginalObject)
                originalObject = default(T);
        }
        
        internal static void FixSerializationForFullPersonName(ref FullPersonName originalFullPersonName)
        {
            if (string.IsNullOrEmpty(originalFullPersonName._namePrefix) && 
                string.IsNullOrEmpty(originalFullPersonName._givenName) && 
                string.IsNullOrEmpty(originalFullPersonName._middleName) &&
                string.IsNullOrEmpty(originalFullPersonName._familyName) &&
                string.IsNullOrEmpty(originalFullPersonName._nameSuffix) &&
                string.IsNullOrEmpty(originalFullPersonName._nickname))
            {
                originalFullPersonName = default(FullPersonName);
            }
        }
    }
}