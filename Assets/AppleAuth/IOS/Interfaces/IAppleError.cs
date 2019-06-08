namespace AppleAuth.IOS.Interfaces
{
    public interface IAppleError
    {
        int Code { get; }
        string Domain { get; }
        string LocalizedDescription { get; }
        string[] LocalizedRecoveryOptions { get; }
        string LocalizedRecoverySuggestion { get; }
        string LocalizedFailureReason { get; }
    }
}
