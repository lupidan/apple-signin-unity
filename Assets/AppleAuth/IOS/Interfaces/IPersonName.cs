namespace AppleAuth.IOS.Interfaces
{
    public interface IPersonName
    {
        string NamePrefix { get; }
        string GivenName { get; }
        string MiddleName { get; }
        string FamilyName { get; }
        string NameSuffix { get; }
        string Nickname { get; }
        IPersonName PhoneticRepresentation { get; }
    }
}
