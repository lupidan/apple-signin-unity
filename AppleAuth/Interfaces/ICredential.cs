namespace AppleAuth.Interfaces
{
    public interface ICredential
    {
        /// <summary>
        /// An identifier associated with the authenticated user
        /// </summary>
        string User { get; }
    }
}
