using AppleAuth.Enums;

namespace AppleAuth
{
    public struct AppleAuthLoginArgs
    {
        public readonly LoginOptions Options;
        public readonly string Nonce;
    
        public AppleAuthLoginArgs(
            LoginOptions options,
            string nonce = null)
        {
            this.Options = options;
            this.Nonce = nonce;
        }
    }
}
