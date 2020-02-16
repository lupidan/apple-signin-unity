namespace AppleAuth.IOS.NativeMessages
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
            if (string.IsNullOrEmpty(originalFullPersonName.NamePrefix) && 
                string.IsNullOrEmpty(originalFullPersonName.GivenName) && 
                string.IsNullOrEmpty(originalFullPersonName.MiddleName) &&
                string.IsNullOrEmpty(originalFullPersonName.FamilyName) &&
                string.IsNullOrEmpty(originalFullPersonName.NameSuffix) &&
                string.IsNullOrEmpty(originalFullPersonName.Nickname) &&
                originalFullPersonName.PhoneticRepresentation == null)
            {
                originalFullPersonName = default(FullPersonName);
            }
        }
    }
}