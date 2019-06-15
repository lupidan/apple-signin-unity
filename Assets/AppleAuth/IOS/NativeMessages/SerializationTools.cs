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
            if (originalArray.Length == 0)
                originalArray = null;
        }

        internal static void FixSerializationForObject<T>(ref T originalObject, bool hasOriginalObject)
        {
            if (!hasOriginalObject)
                originalObject = default(T);
        }
    }
}