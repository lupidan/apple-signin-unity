using System.Text;
using AppleAuth.IOS;
using AppleAuth.IOS.Enums;
using AppleAuth.IOS.Extensions;
using AppleAuth.IOS.Interfaces;
using AppleAuth.IOS.NativeMessages;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private IAppleAuthManager _appleAuthManager = new AppleAuthManager(new PayloadDeserializer(), new ImmediateMessageHandlerScheduler());

    private void OnEnable()
    {
        this._appleAuthManager.LoginSilently(
            credential => Debug.Log(DescribeCredential(credential)),
            error => Debug.Log(DescribeError(error)));

        if (PlayerPrefs.HasKey("Apple ID"))
        {
            this._appleAuthManager.GetCredentialState(PlayerPrefs.GetString("Apple ID"),
                credentialState =>
                {
                    Debug.Log($"Received credential state {credentialState.ToString()}");
                },
                error => Debug.Log(DescribeError(error)));            
        }
    }
    

    public void OnLoginWithAppleButtonPressed()
    {
        this._appleAuthManager.LoginWithAppleId(
            LoginOptions.IncludeEmail,
            credential =>
            {
                Debug.Log(DescribeCredential(credential));
                PlayerPrefs.SetString("Apple ID", credential.User);
                PlayerPrefs.Save();
            },
            error => Debug.Log(DescribeError(error)));
    }

    private static string DescribeError(IAppleError error)
    {
        if (error == null)
            return null;
        
        var sb = new StringBuilder("");
        sb.AppendLine($"Code {error.Code}");
        sb.AppendLine($"Domain: {error.Domain}");
        sb.AppendLine($"Description: {error.LocalizedDescription}");
        sb.AppendLine($"Failure Reason: {error.LocalizedFailureReason}");
        sb.AppendLine($"Recovery Suggestion: {error.LocalizedRecoverySuggestion}");
        sb.AppendLine($"Recovery Options: {(error.LocalizedRecoveryOptions != null ? string.Join(", ", error.LocalizedRecoveryOptions) : "")}");
        return sb.ToString();
    }

    private static string DescribeCredential(ICredential credential)
    {
        var appleIDCredential = credential as IAppleIDCredential;
        var passwordCredential = credential as IPasswordCredential;
        if (appleIDCredential != null)
        {
            var sb = new StringBuilder("IAppleIDCredential");
            sb.AppendLine($"User {appleIDCredential.User}");
            sb.AppendLine($"Identity Token Size: {appleIDCredential.IdentityToken.Length}");
            sb.AppendLine($"Auth Code Size: {appleIDCredential.AuthorizationCode.Length}");
            sb.AppendLine($"State: {appleIDCredential.State}");
            sb.AppendLine($"Email: {appleIDCredential.Email}");
            sb.AppendLine($"Real User Status: {appleIDCredential.RealUserStatus.ToString()}");
            sb.AppendLine($"Auth Scopes: {(appleIDCredential.AuthorizedScopes!= null ? string.Join(", ", appleIDCredential.AuthorizedScopes) : "")}");
            
            if (appleIDCredential.FullName != null)
            {
                sb.AppendLine($"Full Name: START");
                sb.AppendLine($"{DescribePersonName(appleIDCredential.FullName)}");
                sb.AppendLine($"Full Name: END");
            }

            return sb.ToString();
        }
        else if (passwordCredential != null)
        {
            var sb = new StringBuilder("IAppleIDCredential");
            sb.AppendLine($"User {passwordCredential.User}");
            sb.AppendLine($"Password: {passwordCredential.Password}");
            return sb.ToString();
        }

        return null;
    }

    private static string DescribePersonName(IPersonName personName)
    {
        if (personName == null)
            return null;
        
        var sb = new StringBuilder("");
        sb.AppendLine($"NamePrefix {personName.NamePrefix}");
        sb.AppendLine($"GivenName {personName.GivenName}");
        sb.AppendLine($"MiddleName {personName.MiddleName}");
        sb.AppendLine($"FamilyName {personName.FamilyName}");
        sb.AppendLine($"NameSuffix {personName.NameSuffix}");
        sb.AppendLine($"Nickname {personName.Nickname}");
        
        if (personName.PhoneticRepresentation != null)
        {
            sb.AppendLine($"PhoneticRepresentation START");
            sb.AppendLine($"{DescribePersonName(personName.PhoneticRepresentation)}");
            sb.AppendLine($"PhoneticRepresentation END");
        }

        sb.AppendLine(personName.ToLocalizedString(PersonNameFormatterStyle.Long) ?? "No localized name");
        
        return sb.ToString();
    }
}
