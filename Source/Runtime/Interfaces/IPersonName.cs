namespace AppleAuth.Interfaces
{
    /// <summary>
    /// PersonNameComponents
    /// </summary>
    public interface IPersonName
    {
        /// <summary>
        /// The portion of a name’s full form of address that precedes the name itself (for example, “Dr.,” “Mr.,” “Ms.”)
        /// </summary>
        string NamePrefix { get; }

        /// <summary>
        /// Name bestowed upon an individual to differentiate them from other members of a group that share a family name (for example, “Johnathan”)
        /// </summary>
        string GivenName { get; }

        /// <summary>
        /// Secondary name bestowed upon an individual to differentiate them from others that have the same given name (for example, “Maple”)
        /// </summary>
        string MiddleName { get; }

        /// <summary>
        /// Name bestowed upon an individual to denote membership in a group or family. (for example, “Appleseed”)
        /// </summary>
        string FamilyName { get; }

        /// <summary>
        /// The portion of a name’s full form of address that follows the name itself (for example, “Esq.,” “Jr.,” “Ph.D.”)
        /// </summary>
        string NameSuffix { get; }

        /// <summary>
        /// Name substituted for the purposes of familiarity (for example, "Johnny")
        /// </summary>
        string Nickname { get; }

        /// <summary>
        /// The phonetic representation name components of the receiver
        /// </summary>
        IPersonName PhoneticRepresentation { get; }
    }
}
