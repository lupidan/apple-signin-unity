using System;

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

        internal static byte[] GetBytesFromBase64String(string base64String, string fieldName)
        {
            if (base64String == null)
            {
                return null;
            }

            var returnedBytes = default(byte[]);
            try
            {
                returnedBytes = Convert.FromBase64String(base64String);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Received exception while deserializing byte array for " + fieldName);
                Console.WriteLine("Exception: " + exception);
                returnedBytes = null;
            }

            return returnedBytes;
        }
    }
}
