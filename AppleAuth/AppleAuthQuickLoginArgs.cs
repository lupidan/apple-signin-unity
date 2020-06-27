namespace AppleAuth
{
    public struct AppleAuthQuickLoginArgs
    {
        public readonly string Nonce;
        public readonly string State;

        public AppleAuthQuickLoginArgs(string nonce = null, string state = null)
        {
            this.Nonce = nonce;
            this.State = state;
        }
    }
}
