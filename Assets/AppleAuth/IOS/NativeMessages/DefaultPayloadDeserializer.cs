using AppleAuth.IOS.Interfaces;
using UnityEngine;

namespace AppleAuth.IOS.NativeMessages
{
    public class DefaultPayloadDeserializer : IPayloadDeserializer
    {
        public ICredentialStateResponse DeserializeCredentialStateResponse(string payload)
        {
            return JsonUtility.FromJson<DefaultCredentialStateResponse>(payload);
        }
    }
}