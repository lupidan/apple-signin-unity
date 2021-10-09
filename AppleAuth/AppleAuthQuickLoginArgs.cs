namespace AppleAuth
{
    public struct AppleAuthQuickLoginArgs
    {
        public readonly string Nonce;
        public readonly string State;
        public readonly bool ShouldSearchInKeychain;

        public AppleAuthQuickLoginArgs(string nonce = null, string state = null, bool shouldSearchInKeychain = false)
        {
            this.Nonce = nonce;
            this.State = state;
            this.ShouldSearchInKeychain = shouldSearchInKeychain;
        }
    }
}
