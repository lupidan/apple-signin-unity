namespace AppleAuth.IOS
{
    public struct AppleAuthQuickLoginArgs
    {
        public readonly string Nonce;

        public AppleAuthQuickLoginArgs(string nonce = null)
        {
            this.Nonce = nonce;
        }
    }
}
