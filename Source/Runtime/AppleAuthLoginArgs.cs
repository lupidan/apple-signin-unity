using AppleAuth.Enums;

namespace AppleAuth
{
    public struct AppleAuthLoginArgs
    {
        public readonly LoginOptions Options;
        public readonly string Nonce;
        public readonly string State;
    
        public AppleAuthLoginArgs(
            LoginOptions options,
            string nonce = null,
            string state = null)
        {
            this.Options = options;
            this.Nonce = nonce;
            this.State = state;
        }
    }
}
