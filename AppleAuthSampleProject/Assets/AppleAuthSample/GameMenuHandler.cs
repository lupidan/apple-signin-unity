using System;
using System.Text;
using AppleAuth.IOS.Enums;
using AppleAuth.IOS.Extensions;
using AppleAuth.IOS.Interfaces;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class GameMenuHandler
{
    public GameObject Parent;
    public Text AppleUserIdLabel;
    public Text AppleUserCredentialLabel;

    public void SetVisible(bool visible)
    {
        this.Parent.SetActive(visible);
    }
    
    public void SetupAppleData(string appleUserId, ICredential receivedCredential)
    {
        this.AppleUserIdLabel.text = "Apple User ID: " + appleUserId;
        if (receivedCredential == null)
        {
            this.AppleUserCredentialLabel.text = "NO CREDENTIALS RECEIVED\nProbably credential status for " + appleUserId + "was Authorized";
            return;
        }

        var appleIdCredential = receivedCredential as IAppleIDCredential;
        var passwordCredential = receivedCredential as IPasswordCredential;
        if (appleIdCredential != null)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("RECEIVED APPLE ID CREDENTIAL.\nYOU CAN LOGIN/CREATE A USER WITH THIS");
            stringBuilder.AppendLine("<b>Username:</b> " + appleIdCredential.User);
            stringBuilder.AppendLine("<b>Real user status:</b> " + appleIdCredential.RealUserStatus.ToString());

            if (appleIdCredential.State != null)
                stringBuilder.AppendLine("<b>State:</b> " + appleIdCredential.State);

            if (appleIdCredential.IdentityToken != null)
                stringBuilder.AppendLine("<b>Identity token:</b> " + appleIdCredential.IdentityToken.Length + " bytes");

            if (appleIdCredential.AuthorizationCode != null)
                stringBuilder.AppendLine("<b>Authorization Code:</b> " + appleIdCredential.AuthorizationCode.Length + " bytes");

            if (appleIdCredential.AuthorizedScopes != null)
                stringBuilder.AppendLine("<b>Authorized Scopes:</b> " + string.Join(", ", appleIdCredential.AuthorizedScopes));

            if (appleIdCredential.Email != null)
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("<b>EMAIL RECEIVED: YOU WILL ONLY SEE THIS ONCE PER SIGN UP. SEND THIS INFORMATION TO YOUR BACKEND!</b>");
                stringBuilder.AppendLine("<b>You can test this again by revoking credentials in Settings</b>");
                stringBuilder.AppendLine("<b>Email:</b> " + appleIdCredential.Email);
            }

            if (appleIdCredential.FullName != null)
            {
                var fullName = appleIdCredential.FullName;
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("<b>NAME RECEIVED: YOU WILL ONLY SEE THIS ONCE PER SIGN UP. SEND THIS INFORMATION TO YOUR BACKEND!</b>");
                stringBuilder.AppendLine("<b>You can test this again by revoking credentials in Settings</b>");
                stringBuilder.AppendLine("<b>Name:</b> " + fullName.ToLocalizedString());
                stringBuilder.AppendLine("<b>Name (Short):</b> " + fullName.ToLocalizedString(PersonNameFormatterStyle.Short));
                stringBuilder.AppendLine("<b>Name (Medium):</b> " + fullName.ToLocalizedString(PersonNameFormatterStyle.Medium));
                stringBuilder.AppendLine("<b>Name (Long):</b> " + fullName.ToLocalizedString(PersonNameFormatterStyle.Long));
                stringBuilder.AppendLine("<b>Name (Abbreviated):</b> " + fullName.ToLocalizedString(PersonNameFormatterStyle.Abbreviated));

                if (appleIdCredential.FullName.PhoneticRepresentation != null)
                {
                    var phoneticName = appleIdCredential.FullName.PhoneticRepresentation;
                    stringBuilder.AppendLine("<b>Phonetic name:</b> " + phoneticName.ToLocalizedString());
                    stringBuilder.AppendLine("<b>Phonetic name (Short):</b> " + phoneticName.ToLocalizedString(PersonNameFormatterStyle.Short));
                    stringBuilder.AppendLine("<b>Phonetic name (Medium):</b> " + phoneticName.ToLocalizedString(PersonNameFormatterStyle.Medium));
                    stringBuilder.AppendLine("<b>Phonetic name (Long):</b> " + phoneticName.ToLocalizedString(PersonNameFormatterStyle.Long));
                    stringBuilder.AppendLine("<b>Phonetic name (Abbreviated):</b> " + phoneticName.ToLocalizedString(PersonNameFormatterStyle.Abbreviated));
                }
            }

            this.AppleUserCredentialLabel.text = stringBuilder.ToString();
        }
        else if (passwordCredential != null)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("USERNAME/PASSWORD RECEIVED (iCloud?)");
            stringBuilder.AppendLine("<b>Username:</b> " + passwordCredential.User);
            stringBuilder.AppendLine("<b>Password:</b> " + passwordCredential.Password);
            this.AppleUserCredentialLabel.text = stringBuilder.ToString();
        }
        else
        {
            this.AppleUserCredentialLabel.text = "Unknown credentials for user " + receivedCredential.User;
        }
    }
}
