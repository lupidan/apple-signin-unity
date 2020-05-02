using AppleAuth.Interfaces;
using UnityEngine;

namespace AppleAuth.Native
{
    public class PayloadDeserializer : IPayloadDeserializer
    {
        public ICredentialStateResponse DeserializeCredentialStateResponse(string payload)
        {
            return JsonUtility.FromJson<CredentialStateResponse>(payload);
        }

        public ILoginWithAppleIdResponse DeserializeLoginWithAppleIdResponse(string payload)
        {
            return JsonUtility.FromJson<LoginWithAppleIdResponse>(payload);
        }
    }
}