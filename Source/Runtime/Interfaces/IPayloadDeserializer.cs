namespace AppleAuth.Interfaces
{
    public interface IPayloadDeserializer
    {
        ICredentialStateResponse DeserializeCredentialStateResponse(string payload);
        ILoginWithAppleIdResponse DeserializeLoginWithAppleIdResponse(string payload);
    }
}
